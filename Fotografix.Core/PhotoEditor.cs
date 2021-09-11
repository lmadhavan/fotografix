using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Fotografix
{
    public sealed class PhotoEditor : IDisposable
    {
        private readonly CanvasBitmap bitmap;
        private readonly PhotoAdjustment adjustment;

        private PhotoEditor(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
            this.adjustment = new PhotoAdjustment();
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

        public void Draw(CanvasDrawingSession ds)
        {
            adjustment.Render(ds, bitmap);
        }
    }
}
