using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal abstract class AdjustmentNode : IDisposable
    {
        public virtual void Dispose()
        {
        }

        public abstract ICanvasImage GetOutput(ICanvasImage input);
    }
}
