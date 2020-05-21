using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal abstract class AdjustmentNode : CompositionNode, IDisposable
    {
        private ICanvasImage input;

        public virtual void Dispose()
        {
        }

        public ICanvasImage Input
        {
            get
            {
                return input;
            }

            set
            {
                if (input != value)
                {
                    this.input = value;
                    OnInputChanged();
                }
            }
        }

        protected abstract void OnInputChanged();
    }
}
