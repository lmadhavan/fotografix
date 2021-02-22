using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition
{
    public sealed class NullComposableNode : IComposableNode
    {
        public event EventHandler Invalidated;

        public void Dispose()
        {
        }

        public ICanvasImage Compose(ICanvasImage background)
        {
            return background;
        }
    }
}
