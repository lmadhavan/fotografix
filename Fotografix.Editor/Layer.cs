using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Editor
{
    public abstract class Layer : NotifyPropertyChangedBase, IDisposable
    {
        private readonly BlendEffect blendEffect;
        private readonly OpacityEffect opacityEffect;
        private readonly CompositeEffect compositeEffect;

        private string name;
        private bool visible;
        private BlendMode blendMode;
        private float opacity;

        private ICanvasImage content;
        private ICanvasImage background;
        private ICanvasImage output;

        internal Layer()
        {
            this.blendEffect = new BlendEffect();
            this.opacityEffect = new OpacityEffect();

            this.compositeEffect = new CompositeEffect();
            compositeEffect.Sources.Add(null); // this is set through the Background property
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
                    Invalidate();
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
                    Invalidate();
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
                    Invalidate();
                }
            }
        }

        internal event EventHandler Invalidated;

        protected void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        protected ICanvasImage Content
        {
            get
            {
                return content;
            }

            set
            {
                if (content != value)
                {
                    this.content = value;
                    opacityEffect.Source = value;
                    UpdateOutput();
                    Invalidate();
                }
            }
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
                    blendEffect.Background = value;
                    compositeEffect.Sources[0] = value;
                    OnBackgroundChanged();
                    UpdateOutput();
                    Invalidate();
                }
            }
        }

        protected virtual void OnBackgroundChanged()
        {
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

        private void UpdateOutput()
        {
            if (visible && opacity == 1 && blendMode == BlendMode.Normal)
            {
                this.Output = content;
            }
            else if (background == null)
            {
                if (!visible)
                {
                    opacityEffect.Opacity = 0;
                    Invalidate();
                }

                this.Output = opacityEffect;
            }
            else if (!visible || opacity == 0)
            {
                this.Output = Background;
            }
            else if (blendMode == BlendMode.Normal /* && opacity != 1 */)
            {
                this.Output = compositeEffect;
            }
            else if (opacity == 1 /* && blendMode != BlendMode.Normal */)
            {
                blendEffect.Foreground = content;
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
