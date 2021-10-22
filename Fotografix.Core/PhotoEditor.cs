using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace Fotografix
{
    public sealed class PhotoEditor : NotifyPropertyChangedBase, IDisposable
    {
        private static readonly ThumbnailRenderer ThumbnailRenderer = new ThumbnailRenderer(512);

        private readonly Photo photo;
        private readonly CanvasBitmap bitmap;
        private PhotoAdjustment adjustment;
        private State state;
        private bool showOriginal;

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

        public bool ShowOriginal
        {
            get => showOriginal;

            set
            {
                if (SetProperty(ref showOriginal, value))
                {
                    Invalidate();
                }
            }
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            ds.DrawImage(showOriginal ? bitmap : adjustment.Output);
        }

        public CanvasRenderTarget CreateCompatibleRenderTarget()
        {
            return new CanvasRenderTarget(bitmap, bitmap.Size);
        }

        public void ResetAdjustment()
        {
            this.Adjustment = new PhotoAdjustment { Source = bitmap };
            this.state = State.Reset;
        }

        private async Task SaveInternalAsync()
        {
            using (var thumbnail = ThumbnailRenderer.RenderThumbnail(image: adjustment.Output, resourceCreator: bitmap))
            using (var softwareBitmap = await SoftwareBitmap.CreateCopyFromSurfaceAsync(thumbnail))
            {
                await photo.SaveAdjustmentAsync(adjustment, softwareBitmap);
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

        private enum State
        {
            Clean,
            Dirty,
            Reset
        }
    }
}
