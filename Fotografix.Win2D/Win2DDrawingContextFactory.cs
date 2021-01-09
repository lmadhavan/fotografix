using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;

namespace Fotografix.Win2D
{
    public sealed class Win2DDrawingContextFactory : IDrawingContextFactory
    {
        private readonly ICanvasResourceCreator resourceCreator = CanvasDevice.GetSharedDevice();

        public IDrawingContext CreateDrawingContext(Bitmap bitmap)
        {
            Win2DBitmap win2DBitmap = new Win2DBitmap(bitmap, resourceCreator);

            Win2DDrawingContext dc = win2DBitmap.CreateDrawingContext();
            dc.Disposed += (s, e) =>
            {
                win2DBitmap.UpdateSource();
                win2DBitmap.Dispose();
            };

            return dc;
        }
    }
}
