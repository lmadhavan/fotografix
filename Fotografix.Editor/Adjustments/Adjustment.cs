using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Editor.Adjustments
{
    public abstract class Adjustment : NotifyPropertyChangedBase, IDisposable
    {
        private readonly BlendEffect blendEffect;
        private readonly OpacityEffect opacityEffect;
        private readonly CompositeEffect compositeEffect;

        private string name;
        private bool visible;
        private BlendMode blendMode;
        private float opacity;

        private ICanvasImage input;
        private ICanvasImage rawOutput;
        private ICanvasImage output;

        protected Adjustment()
        {
            this.blendEffect = new BlendEffect();
            this.opacityEffect = new OpacityEffect();

            this.compositeEffect = new CompositeEffect();
            compositeEffect.Sources.Add(null); // index 0 is set via Input property
            compositeEffect.Sources.Add(opacityEffect);

            this.visible = true;
            this.blendMode = BlendMode.Normal;
            this.opacity = 1;
        }

        public virtual void Dispose()
        {
            compositeEffect.Dispose();
            opacityEffect.Dispose();
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
                    if (blendMode != BlendMode.Normal)
                    {
                        blendEffect.Mode = Enum.Parse<BlendEffectMode>(Enum.GetName(typeof(BlendMode), blendMode));
                    }

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
                    opacityEffect.Opacity = opacity;
                    UpdateOutput();
                }
            }
        }

        internal ICanvasImage Input
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
                    blendEffect.Background = value;
                    compositeEffect.Sources[0] = value;
                    OnInputChanged();
                }
            }
        }

        protected ICanvasImage RawOutput
        {
            get
            {
                return rawOutput;
            }

            set
            {
                if (rawOutput != value)
                {
                    this.rawOutput = value;
                    opacityEffect.Source = value;
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

        protected virtual void OnInputChanged()
        {
        }

        private void UpdateOutput()
        {
            if (opacity == 0 || visible == false)
            {
                this.Output = Input;
            }
            else if (blendMode == BlendMode.Normal && opacity == 1)
            {
                this.Output = RawOutput;
            }
            else if (blendMode == BlendMode.Normal /* && opacity != 1 */)
            {
                this.Output = compositeEffect;
            }
            else if (opacity == 1 /* && blendMode != BlendMode.Normal */)
            {
                blendEffect.Foreground = RawOutput;
                this.Output = blendEffect;
            }
            else /* blendMode != BlendMode.Normal && opacity != 1 */
            {
                blendEffect.Foreground = opacityEffect;
                this.Output = blendEffect;
            }
        }
    }
}