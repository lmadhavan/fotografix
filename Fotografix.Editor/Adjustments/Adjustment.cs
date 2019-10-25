using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Editor.Adjustments
{
    public abstract class Adjustment : NotifyPropertyChangedBase, IDisposable
    {
        private ICanvasImage input;

        public abstract void Dispose();

        internal ICanvasImage Input
        {
            get
            {
                return input;
            }

            set
            {
                this.input = value;
                OnInputChanged();
            }
        }

        internal abstract ICanvasImage Output { get; }

        protected virtual void OnInputChanged()
        {
        }
    }
}