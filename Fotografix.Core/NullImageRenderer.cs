namespace Fotografix
{
    public sealed class NullImageRenderer : IImageRenderer
    {
        public Bitmap Render(Image image)
        {
            return new Bitmap(image.Size);
        }
    }
}
