using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace Fotografix.Win2D.Drawing
{
    public interface IWin2DDrawable : IDrawable
    {
        void Draw(CanvasDrawingSession ds, Rectangle bounds);
    }
}