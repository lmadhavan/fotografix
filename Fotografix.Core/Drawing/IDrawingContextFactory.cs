namespace Fotografix.Drawing
{
    public interface IDrawingContextFactory
    {
        IDrawingContext CreateDrawingContext(Bitmap bitmap);
    }
}
