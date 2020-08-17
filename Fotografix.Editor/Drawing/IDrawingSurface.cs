using Fotografix.Drawing;

namespace Fotografix.Editor.Drawing
{
    public interface IDrawingSurface
    {
        void BeginDrawing(IDrawable drawable);
        void EndDrawing(IDrawable drawable);
    }
}
