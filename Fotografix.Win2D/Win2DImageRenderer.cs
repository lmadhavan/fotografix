using Fotografix.Editor;

namespace Fotografix.Win2D
{
    public sealed class Win2DImageRenderer : IImageRenderer
    {
        public Bitmap Render(Image image)
        {
            using (Win2DCompositor compositor = new Win2DCompositor(image, new Viewport(image.Size), 0))
            {
                return compositor.ToBitmap();
            }
        }
    }
}
