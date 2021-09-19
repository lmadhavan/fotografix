using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Fotografix
{
    public sealed class PhotoEditor : NotifyPropertyChangedBase, IDisposable
    {
        private readonly CanvasBitmap bitmap;
        private PhotoAdjustment adjustment;

        private PhotoEditor(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
            ResetAdjustment();
        }

        public void Dispose()
        {
            adjustment.Dispose();
            bitmap.Dispose();
        }

        public static async Task<PhotoEditor> CreateAsync(Photo photo)
        {
            using (var stream = await photo.Content.OpenReadAsync())
            {
                var bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
                return new PhotoEditor(bitmap);
            }
        }

        public Size Size => bitmap.Size;
        public PhotoAdjustment Adjustment => adjustment;
        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            adjustment.Render(ds, bitmap);
        }

        public void ResetAdjustment()
        {
            adjustment?.Dispose();
            this.adjustment = new PhotoAdjustment();
            adjustment.PropertyChanged += (s, e) => Invalidate();
            RaisePropertyChanged(nameof(Adjustment));
            Invalidate();
        }
        
        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
