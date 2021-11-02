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
        private SaveState saveState;

        private PhotoEditor(Photo photo, CanvasBitmap bitmap)
        {
            this.photo = photo;
            this.bitmap = bitmap;
            this.saveState = SaveState.Clean;
        }

        public void Dispose()
        {
            adjustment.Dispose();
            bitmap.Dispose();
        }

        public static Task<PhotoEditor> CreateAsync(Photo photo)
        {
            return CreateAsync(photo, CanvasDevice.GetSharedDevice());
        }

        public static async Task<PhotoEditor> CreateAsync(Photo photo, ICanvasResourceCreator canvasResourceCreator)
        {
            using (var stream = await photo.OpenContentAsync())
            {
                var bitmap = await CanvasBitmap.LoadAsync(canvasResourceCreator, stream);
                var adjustment = await photo.LoadAdjustmentAsync();
                adjustment.Source = bitmap;

                var editor = new PhotoEditor(photo, bitmap);
                editor.SetAdjustment(adjustment);
                return editor;
            }
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
                    PreserveSaveState(() => adjustment.Enabled = value);
                    RaisePropertyChanged();
                }
            }
        }

        public Size RenderSize => adjustment.GetOutputSize(resourceCreator: bitmap);

        public float RenderScale
        {
            get => adjustment.RenderScale;

            set
            {
                if (adjustment.RenderScale != value)
                {
                    PreserveSaveState(() => adjustment.RenderScale = value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(RenderSize));
                }
            }
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            ds.DrawImage(adjustment.Output);
        }

        public void SetRenderSize(Size size)
        {
            this.RenderScale = (float)Math.Min(size.Width / bitmap.Size.Width, size.Height / bitmap.Size.Height);
        }

        public void ResetAdjustment()
        {
            SetAdjustment(new PhotoAdjustment { Source = bitmap, RenderScale = adjustment.RenderScale });
            this.saveState = SaveState.PendingDelete;
        }

        public Task SaveAsync()
        {
            Task result;

            switch (saveState)
            {
                case SaveState.Clean:
                    result = Task.CompletedTask;
                    break;

                case SaveState.Dirty:
                    result = SaveInternalAsync();
                    break;

                case SaveState.PendingDelete:
                    result = photo.DeleteAdjustmentAsync();
                    break;

                default:
                    throw new InvalidOperationException();
            }

            this.saveState = SaveState.Clean;
            return result;
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

        private async Task SaveInternalAsync()
        {
            using (var thumbnail = await ExportToSoftwareBitmapAsync(ThumbnailSize))
            {
                await photo.SaveAdjustmentAsync(adjustment, thumbnail);
            }
        }

        public CanvasBitmap ExportToCanvasBitmap(int? maxDimension = null)
        {
            this.AdjustmentEnabled = true;

            var originalRenderScale = adjustment.RenderScale;
            adjustment.RenderScale = 1;

            var originalBounds = adjustment.Output.GetBounds(resourceCreator: bitmap);
            var scaledSize = ScaleDimensions(new Size(originalBounds.Width, originalBounds.Height), maxDimension);
            var rt = new CanvasRenderTarget(resourceCreator: bitmap, size: scaledSize);

            using (var ds = rt.CreateDrawingSession())
            {
                ds.DrawImage(adjustment.Output, rt.Bounds, originalBounds);
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

        private void SetAdjustment(PhotoAdjustment value)
        {
            adjustment?.Dispose();
            this.adjustment = value;
            adjustment.Changed += OnAdjustmentChanged;
            RaisePropertyChanged(nameof(Adjustment));
            RaisePropertyChanged(nameof(AdjustmentEnabled));
            Invalidate();
        }

        private void OnAdjustmentChanged(object sender, EventArgs e)
        {
            this.saveState = SaveState.Dirty;
            Invalidate();
        }

        private void PreserveSaveState(Action action)
        {
            var oldSaveState = saveState;
            action();
            this.saveState = oldSaveState;
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

        private enum SaveState
        {
            Clean,
            Dirty,
            PendingDelete
        }
    }
}
