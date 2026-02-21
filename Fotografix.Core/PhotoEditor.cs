using Fotografix.Export;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Fotografix
{
    public sealed class PhotoEditor : IRenderScaleProvider, IExportable, IDisposable
    {
        private readonly IPhoto photo;
        private readonly CanvasBitmap bitmap;
        private readonly Transform2DEffect histogramTransform;
        private PhotoAdjustment adjustment;
        private bool dirty;

        private PhotoEditor(IPhoto photo, CanvasBitmap bitmap, PhotoMetadata metadata)
        {
            this.photo = photo;
            this.bitmap = bitmap;
            this.Metadata = metadata;
            this.histogramTransform = new Transform2DEffect { CacheOutput = true };
        }

        public void Dispose()
        {
            histogramTransform.Dispose();
            adjustment.Dispose();
            bitmap.Dispose();
        }

        public static Task<PhotoEditor> CreateAsync(IPhoto photo)
        {
            return CreateAsync(photo, CanvasDevice.GetSharedDevice());
        }

        public static async Task<PhotoEditor> CreateAsync(IPhoto photo, ICanvasResourceCreator canvasResourceCreator)
        {
            var bitmap = await photo.LoadBitmapAsync(canvasResourceCreator);
            var adjustment = await photo.LoadAdjustmentAsync();
            var metadata = await photo.GetMetadataAsync();

            var editor = new PhotoEditor(photo, bitmap, metadata);
            editor.CanRevert = adjustment != null;
            editor.SetAdjustment(adjustment);
            return editor;
        }

        public int ThumbnailSize { get; set; } = 512;

        public string Name => photo.Name;
        public IPhotoAdjustment Adjustment => adjustment;
        public PhotoMetadata Metadata { get; }

        public bool AdjustmentEnabled
        {
            get => adjustment.Enabled;

            set
            {
                if (adjustment.Enabled != value)
                {
                    PreserveDirtyState(() => adjustment.Enabled = value);
                }
            }
        }

        public void Draw(CanvasDrawingSession ds)
        {
            ds.DrawImage(adjustment.Output);
        }

        public event EventHandler Invalidated;

        public Size OrientedSize => adjustment.GetOrientedSize();
        public Size RenderSize => adjustment.GetOutputSize();

        public float RenderScale
        {
            get => adjustment.RenderScale;

            set
            {
                if (adjustment.RenderScale != value)
                {
                    PreserveDirtyState(() => adjustment.RenderScale = value);
                }
            }
        }

        public void ScaleToFit(Size size)
        {
            Size contentSize = OrientedSize;

            if (adjustment.Crop.HasValue)
            {
                contentSize = new Size(adjustment.Crop.Value.Width, adjustment.Crop.Value.Height);
            }

            this.RenderScale = (float)Math.Min(size.Width / contentSize.Width, size.Height / contentSize.Height);
        }

        public bool CanRevert { get; private set; }

        public async Task RevertAsync()
        {
            SetAdjustment(await photo.LoadAdjustmentAsync());
            this.dirty = false;
        }

        public void Reset()
        {
            SetAdjustment(new PhotoAdjustment());
            this.dirty = true;
        }

        public async Task SaveAsync()
        {
            if (dirty)
            {
                using (var thumbnail = await ExportToSoftwareBitmapAsync(ThumbnailSize))
                {
                    await photo.SaveAdjustmentAsync(adjustment, thumbnail);
                }

                this.dirty = false;
            }
        }

        public async Task<StorageFile> ExportAsync(ExportOptions options)
        {
            using (var bitmap = await ExportToSoftwareBitmapAsync(options.MaxDimension))
            {
                var file = await options.DestinationFolder.CreateFileAsync(Path.GetFileNameWithoutExtension(photo.Name) + ".jpg", CreationCollisionOption.GenerateUniqueName);

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encodingOptions = new Dictionary<string, BitmapTypedValue>
                    {
                        ["ImageQuality"] = new BitmapTypedValue(options.Quality, PropertyType.Single)
                    };

                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream, encodingOptions);
                    encoder.SetSoftwareBitmap(bitmap);
                    await encoder.FlushAsync();
                }

                return file;
            }
        }

        public CanvasBitmap ExportToCanvasBitmap(int? maxDimension = null)
        {
            this.AdjustmentEnabled = true;

            var originalRenderScale = adjustment.RenderScale;
            adjustment.RenderScale = 1;

            var size = adjustment.GetOutputSize();
            var scaledSize = ScaleDimensions(size, maxDimension);
            var rt = new CanvasRenderTarget(ResourceCreator, size: scaledSize);

            using (var ds = rt.CreateDrawingSession())
            {
                ds.DrawImage(adjustment.Output, rt.Bounds, new Rect(new Point(), size));
            }

            adjustment.RenderScale = originalRenderScale;
            return rt;
        }

        public async Task<SoftwareBitmap> ExportToSoftwareBitmapAsync(int? maxDimension = null)
        {
            using (var bitmap = ExportToCanvasBitmap(maxDimension))
            {
                return await SoftwareBitmap.CreateCopyFromSurfaceAsync(bitmap);
            }
        }

        public Histogram ComputeHistogram()
        {
            var bounds = adjustment.Output.GetBounds(ResourceCreator);
            var scaledSize = ScaleDimensions(new Size(bounds.Width, bounds.Height), ThumbnailSize);

            histogramTransform.TransformMatrix = Matrix3x2.CreateScale((float)(scaledSize.Width / bounds.Width));
            histogramTransform.Source = adjustment.Output;
            return Histogram.Compute(histogramTransform, new Rect(0, 0, scaledSize.Width, scaledSize.Height), ResourceCreator);
        }

        private ICanvasResourceCreatorWithDpi ResourceCreator => bitmap;

        private void SetAdjustment(PhotoAdjustment value)
        {
            if (value == null)
            {
                value = new PhotoAdjustment();
            }

            value.Source = bitmap;

            if (adjustment != null)
            {
                value.RenderScale = adjustment.RenderScale;
                adjustment.Dispose();
            }

            this.adjustment = value;
            adjustment.Changed += OnAdjustmentChanged;
            Invalidate();
        }

        private void OnAdjustmentChanged(object sender, EventArgs e)
        {
            this.dirty = true;
            Invalidate();
        }

        private void PreserveDirtyState(Action action)
        {
            var oldDirty = this.dirty;
            action();
            this.dirty = oldDirty;
        }

        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        public static Size ScaleDimensions(Size originalSize, int? maxDimension)
        {
            if (maxDimension == null)
            {
                return originalSize;
            }

            int d = maxDimension.Value;

            if (originalSize.Width <= d && originalSize.Height <= d)
            {
                return originalSize;
            }

            if (originalSize.Width > originalSize.Height)
            {
                return new Size(d, d * originalSize.Height / originalSize.Width);
            }
            else
            {
                return new Size(d * originalSize.Width / originalSize.Height, d);
            }
        }
    }
}
