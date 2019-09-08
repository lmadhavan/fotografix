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
        private BlackAndWhiteAdjustment adjustment;
        private ICanvasImage output;

        public Image(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
            this.output = bitmap;
        }

        public void Dispose()
        {
            adjustment?.Dispose();
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
            drawingSession.DrawImage(output);
        }

        public void ApplyBlackAndWhiteAdjustment(BlendMode blendMode = BlendMode.Normal)
        {
            adjustment?.Dispose();
            this.adjustment = new BlackAndWhiteAdjustment(bitmap, blendMode);
            this.output = adjustment.Output;
        }
    }
}