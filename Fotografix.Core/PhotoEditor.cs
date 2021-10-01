using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Fotografix
{
    public sealed class PhotoEditor : NotifyPropertyChangedBase, IDisposable
    {
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
                    result = photo.SaveAdjustmentAsync(adjustment);
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
                adjustment.PropertyChanged += OnAdjustmentPropertyChanged;
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
            if (showOriginal)
            {
                ds.DrawImage(bitmap);
            }
            else
            {
                adjustment.Render(ds, bitmap);
            }
        }

        public void ResetAdjustment()
        {
            this.Adjustment = new PhotoAdjustment();
            this.state = State.Reset;
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
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
