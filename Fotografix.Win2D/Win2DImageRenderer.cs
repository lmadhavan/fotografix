using Fotografix.Editor;

namespace Fotografix.Win2D
{
    public sealed class Win2DImageRenderer : IImageRenderer
    {
        private static readonly Win2DCompositorSettings CompositorSettings = new Win2DCompositorSettings();

        public Bitmap Render(Image image)
        {
            using (Win2DCompositor compositor = new Win2DCompositor(image, new Viewport(image.Size), CompositorSettings))
            {
                return compositor.ToBitmap();
            }
        }
    }
}
