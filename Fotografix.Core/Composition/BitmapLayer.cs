using Microsoft.Graphics.Canvas;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.Composition
{
    public sealed class BitmapLayer : Layer
    {
        private readonly CanvasBitmap bitmap;

        public BitmapLayer(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
            this.Content = bitmap;
        }

        public BitmapLayer(ICanvasResourceCreator resourceCreator, int width, int height)
            : this(new CanvasRenderTarget(resourceCreator, width, height, 96))
        {
        }

        public static async Task<BitmapLayer> LoadAsync(ICanvasResourceCreator resourceCreator, StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(resourceCreator, stream);
                return new BitmapLayer(bitmap) { Name = file.DisplayName };
            }
        }

        public int Width => (int)bitmap.SizeInPixels.Width;
        public int Height => (int)bitmap.SizeInPixels.Height;
    }
}
