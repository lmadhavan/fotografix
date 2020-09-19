using Fotografix.Drawing;

namespace Fotografix.Win2D
{
    public sealed class Win2DDrawingContextFactory : IDrawingContextFactory
    {
        public IDrawingContext CreateDrawingContext(Bitmap bitmap)
        {
            return new Win2DBitmapDrawingContext(bitmap);
        }
    }
}
