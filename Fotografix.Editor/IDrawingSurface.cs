namespace Fotografix.Editor
{
    public interface IDrawingSurface
    {
        void BeginDrawing(BrushStroke brushStroke);
        void EndDrawing(BrushStroke brushStroke);
    }
}
