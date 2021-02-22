using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition
{
    internal interface IComposableNode : IDisposable
    {
        ICanvasImage Compose(ICanvasImage background);

        event EventHandler Invalidated;
    }
}
