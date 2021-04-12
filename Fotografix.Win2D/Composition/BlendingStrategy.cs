using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition
{
    internal interface IBlendingStrategy : IDisposable
    {
        ICanvasImage Blend(Layer layer, ICanvasImage background);

        event EventHandler Invalidated;
    }
}
