using Microsoft.Graphics.Canvas;
using System;
using Windows.Foundation;

namespace Fotografix.Win2D.Composition
{
    internal interface IDrawableNode : IDisposable
    {
        void Draw(CanvasDrawingSession ds, Rect imageBounds);
    }
}
