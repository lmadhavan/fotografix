using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.Graphics.Effects;

namespace Fotografix.Editor.Adjustments
{
    public abstract class Adjustment : NotifyPropertyChangedBase, IDisposable
    {
        private readonly BlendEffect blendEffect;

        private string name;
        private BlendMode blendMode;
        private ICanvasImage output;

        protected Adjustment()
        {
            this.blendEffect = new BlendEffect();
        }

        public virtual void Dispose()
        {
            blendEffect.Dispose();
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                SetValue(ref name, value);
            }
        }

        public BlendMode BlendMode
        {
            get
            {
                return blendMode;
            }

            set
            {
                if (SetValue(ref blendMode, value))
                {
                    if (blendMode != BlendMode.Normal)
                    {
                        blendEffect.Mode = Enum.Parse<BlendEffectMode>(Enum.GetName(typeof(BlendMode), blendMode));
                    }

                    UpdateOutput();
                }
            }
        }

        internal IGraphicsEffectSource Input
        {
            get
            {
                return blendEffect.Background;
            }

            set
            {
                blendEffect.Background = value;
                OnInputChanged();
            }
        }

        protected ICanvasImage RawOutput
        {
            get
            {
                return (ICanvasImage)blendEffect.Foreground;
            }

            set
            {
                blendEffect.Foreground = value;
                UpdateOutput();
            }
        }

        internal ICanvasImage Output
        {
            get
            {
                return output;
            }

            private set
            {
                if (output != value)
                {
                    this.output = value;
                    OutputChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        internal event EventHandler OutputChanged;

        protected virtual void OnInputChanged()
        {
        }

        private void UpdateOutput()
        {
            if (blendMode == BlendMode.Normal)
            {
                this.Output = RawOutput;
            }
            else
            {
                this.Output = blendEffect;
            }
        }
    }
}