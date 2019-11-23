using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Fotografix.Win2D
{
    public sealed class Win2DBitmapFactory : IBitmapFactory
    {
        private readonly ICanvasResourceCreator resourceCreator;

        public Win2DBitmapFactory(ICanvasResourceCreator resourceCreator)
        {
            this.resourceCreator = resourceCreator;
        }

        public IBitmap CreateBitmap(Size size)
        {
            CanvasRenderTarget bitmap = new CanvasRenderTarget(resourceCreator, size.Width, size.Height, 96);
            return new BitmapNode(bitmap);
        }

        public async Task<IBitmap> LoadBitmapAsync(Stream stream)
        {
            CanvasBitmap bitmap = await CanvasBitmap.LoadAsync(resourceCreator, stream.AsRandomAccessStream());
            return new BitmapNode(bitmap);
        }
    }
}
