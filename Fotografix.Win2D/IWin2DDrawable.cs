using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace Fotografix.Win2D
{
    public interface IWin2DDrawable
    {
        Size Size { get; }
        void Draw(CanvasDrawingSession ds);
    }
}
