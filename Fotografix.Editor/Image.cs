using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Editor
{
    public sealed class Image : IDisposable
    {
        private readonly CanvasBitmap bitmap;
        private GrayscaleEffect effect;

        public Image(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public void Dispose()
        {
            effect?.Dispose();
            bitmap.Dispose();
        }

        public int Width => (int)bitmap.SizeInPixels.Width;
        public int Height => (int)bitmap.SizeInPixels.Height;

        public static async Task<Image> LoadAsync(StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                var bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
                return new Image(bitmap);
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            drawingSession.DrawImage((ICanvasImage)effect ?? bitmap);
        }

        public void ApplyBlackAndWhiteAdjustment()
        {
            if (effect == null)
            {
                this.effect = new GrayscaleEffect() { Source = bitmap };
            }
        }
    }
}