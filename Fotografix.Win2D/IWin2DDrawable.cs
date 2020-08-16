using Microsoft.Graphics.Canvas;

namespace Fotografix.Win2D
{
    public interface IWin2DDrawable : IDrawable
    {
        void Draw(CanvasDrawingSession ds);
    }
}