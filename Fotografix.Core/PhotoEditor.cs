using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Fotografix
{
    public sealed class PhotoEditor : IRenderScaleProvider, IDisposable
    {
        private readonly IPhoto photo;
        private readonly CanvasBitmap bitmap;
        private readonly Transform2DEffect histogramTransform;
        private PhotoAdjustment adjustment;
        private bool dirty;

        private PhotoEditor(IPhoto photo, CanvasBitmap bitmap)
        {
            this.photo = photo;
            this.bitmap = bitmap;
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

            var editor = new PhotoEditor(photo, bitmap);
            editor.CanRevert = adjustment != null;
            editor.SetAdjustment(adjustment);
            return editor;
        }

        public int ThumbnailSize { get; set; } = 512;

        public IPhotoAdjustment Adjustment => adjustment;

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

        public Size OriginalSize => bitmap.Size;
        public Size RenderSize => adjustment.GetOutputSize(ResourceCreator);

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
            Size photoSize = bitmap.Size;

            if (adjustment.Crop.HasValue)
            {
                photoSize = new Size(adjustment.Crop.Value.Width, adjustment.Crop.Value.Height);
            }

            this.RenderScale = (float)Math.Min(size.Width / photoSize.Width, size.Height / photoSize.Height);
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

        public async Task<StorageFile> ExportAsync(StorageFolder folder)
        {
            using (var bitmap = await ExportToSoftwareBitmapAsync())
            {
                var file = await folder.CreateFileAsync(photo.Name + ".jpg", CreationCollisionOption.GenerateUniqueName);

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
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

            var bounds = adjustment.Output.GetBounds(ResourceCreator);
            var scaledSize = ScaleDimensions(new Size(bounds.Width, bounds.Height), maxDimension);
            var rt = new CanvasRenderTarget(ResourceCreator, size: scaledSize);

            using (var ds = rt.CreateDrawingSession())
            {
                ds.DrawImage(adjustment.Output, rt.Bounds, bounds);
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
