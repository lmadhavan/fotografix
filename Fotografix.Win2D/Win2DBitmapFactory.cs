using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace Fotografix.Win2D
{
    public sealed class Win2DBitmapFactory : IBitmapFactory
    {
        private readonly ICanvasResourceCreator resourceCreator;

        public Win2DBitmapFactory() : this(CanvasDevice.GetSharedDevice())
        {
        }

        public Win2DBitmapFactory(ICanvasResourceCreator resourceCreator)
        {
            this.resourceCreator = resourceCreator;
        }

        public Bitmap CreateBitmap(Size size)
        {
            return new Win2DBitmap(size, resourceCreator);
        }
    }
}
