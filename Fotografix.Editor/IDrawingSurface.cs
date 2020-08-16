namespace Fotografix.Editor
{
    public interface IDrawingSurface
    {
        void BeginDrawing(IDrawable drawable);
        void EndDrawing(IDrawable drawable);
    }
}
