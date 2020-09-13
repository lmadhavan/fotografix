namespace Fotografix.Drawing
{
    public interface IDrawable : INotifyContentChanged
    {
        void Draw(IDrawingContext drawingContext);
    }
}