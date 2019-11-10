using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Composition
{
    public abstract class Layer : NotifyPropertyChangedBase, IDisposable
    {
        private readonly BlendEffect blendEffect;

        private string name;
        private bool visible;
        private BlendMode blendMode;
        private float opacity;

        private ICanvasImage background;
        private ICanvasImage output;

        internal Layer()
        {
            this.blendEffect = new BlendEffect();

            this.visible = true;
            this.blendMode = BlendMode.Normal;
            this.opacity = 1;
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

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                if (SetValue(ref visible, value))
                {
                    UpdateOutput();
                }
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
                    UpdateOutput();
                }
            }
        }

        public float Opacity
        {
            get
            {
                return opacity;
            }

            set
            {
                if (SetValue(ref opacity, value))
                {
                    UpdateOutput();
                }
            }
        }

        public event EventHandler Invalidated;

        protected void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        internal ICanvasImage Background
        {
            get
            {
                return background;
            }

            set
            {
                if (background != value)
                {
                    this.background = value;
                    UpdateOutput();
                }
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

        protected void UpdateOutput()
        {
            this.Output = ResolveOutput(background);
            Invalidate();
        }

        protected abstract ICanvasImage ResolveOutput(ICanvasImage background);

        protected ICanvasImage Blend(ICanvasImage foreground, ICanvasImage background)
        {
            blendEffect.Mode = Enum.Parse<BlendEffectMode>(Enum.GetName(typeof(BlendMode), blendMode));
            blendEffect.Foreground = foreground;
            blendEffect.Background = background;
            return blendEffect;
        }
    }
}
