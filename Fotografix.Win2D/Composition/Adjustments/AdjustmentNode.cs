using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal abstract class AdjustmentNode : CompositionNode, IDisposable
    {
        private readonly Adjustment adjustment;
        private ICanvasImage input;

        protected AdjustmentNode(Adjustment adjustment)
        {
            this.adjustment = adjustment;
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;
        }

        public virtual void Dispose()
        {
            adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
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

        protected virtual void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Invalidate();
        }
    }
}
