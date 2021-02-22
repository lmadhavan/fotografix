using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace Fotografix.Win2D.Composition
{
    public sealed class NullDrawableNode : IDrawableNode
    {
        public void Dispose()
        {
        }

        public void Draw(CanvasDrawingSession ds, Rect imageBounds)
        {
        }
    }
}
