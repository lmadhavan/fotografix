namespace Fotografix.Win2D
{
    public sealed class Win2DImageRenderer : IImageRenderer
    {
        public Bitmap Render(Image image)
        {
            using (Win2DCompositor compositor = new Win2DCompositor(image))
            {
                return compositor.ToBitmap();
            }
        }
    }
}
