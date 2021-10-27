using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Fotografix
{
    public sealed class PhotoEditor : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Photo photo;
        private readonly CanvasBitmap bitmap;
        private PhotoAdjustment adjustment;
        private State state;

        private PhotoEditor(Photo photo, CanvasBitmap bitmap)
        {
            this.photo = photo;
            this.bitmap = bitmap;
            this.state = State.Clean;
        }

        public void Dispose()
        {
            adjustment.Dispose();
            bitmap.Dispose();
        }

        public static async Task<PhotoEditor> CreateAsync(Photo photo)
        {
            using (var stream = await photo.OpenContentAsync())
            {
                var bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
                var adjustment = await photo.LoadAdjustmentAsync();
                adjustment.Source = bitmap;
                return new PhotoEditor(photo, bitmap) { Adjustment = adjustment };
            }
        }

        public int ThumbnailSize { get; set; } = 512;
        public Size Size => bitmap.Size;

        public PhotoAdjustment Adjustment
        {
            get => adjustment;

            private set
            {
                adjustment?.Dispose();
                this.adjustment = value;
                adjustment.Changed += OnAdjustmentChanged;
                RaisePropertyChanged(nameof(Adjustment));
                Invalidate();
            }
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds, bool hideAdjustment = false)
        {
            ds.DrawImage(hideAdjustment ? bitmap : adjustment.Output);
        }

        public void ResetAdjustment()
        {
            this.Adjustment = new PhotoAdjustment { Source = bitmap };
            this.state = State.Reset;
        }

        public Task SaveAsync()
        {
            Task result;

            switch (state)
            {
                case State.Clean:
                    result = Task.CompletedTask;
                    break;

                case State.Dirty:
                    result = SaveInternalAsync();
                    break;

                case State.Reset:
                    result = photo.DeleteAdjustmentAsync();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            this.state = State.Clean;
            return result;
        }

        public async Task<StorageFile> ExportAsync(StorageFolder folder)
        {
            using (var bitmap = await RenderToSoftwareBitmapAsync())
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

        private async Task SaveInternalAsync()
        {
            using (var thumbnail = await RenderToSoftwareBitmapAsync(ThumbnailSize))
            {
                await photo.SaveAdjustmentAsync(adjustment, thumbnail);
            }
        }

        public CanvasBitmap RenderToCanvasBitmap(int? maxDimension = null)
        {
            var originalBounds = adjustment.Output.GetBounds(resourceCreator: bitmap);
            var scaledSize = ScaleDimensions(new Size(originalBounds.Width, originalBounds.Height), maxDimension);
            var rt = new CanvasRenderTarget(resourceCreator: bitmap, size: scaledSize);

            using (var ds = rt.CreateDrawingSession())
            {
                ds.DrawImage(adjustment.Output, rt.Bounds, originalBounds);
            }

            return rt;
        }

        public async Task<SoftwareBitmap> RenderToSoftwareBitmapAsync(int? maxDimension = null)
        {
            using (var bitmap = RenderToCanvasBitmap(maxDimension))
            {
                return await SoftwareBitmap.CreateCopyFromSurfaceAsync(bitmap);
            }
        }

        private void OnAdjustmentChanged(object sender, EventArgs e)
        {
            this.state = State.Dirty;
            Invalidate();
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

        private enum State
        {
            Clean,
            Dirty,
            Reset
        }
    }
}
