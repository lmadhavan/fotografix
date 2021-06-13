using Fotografix.Drawing;

namespace Fotografix
{
    public interface IGraphicsDevice
    {
        IDrawingContext CreateDrawingContext(Bitmap bitmap);
    }
}
