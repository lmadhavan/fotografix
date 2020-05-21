using System;
using System.ComponentModel;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition
{
    internal abstract class LayerNode : IDisposable
    {
        private readonly Layer layer;
        private readonly BlendEffect blendEffect;

        private ICanvasImage background;
        private ICanvasImage output;

        protected LayerNode(Layer layer)
        {
            this.layer = layer;
            this.blendEffect = new BlendEffect();
            layer.PropertyChanged += OnLayerPropertyChanged;
        }

        public virtual void Dispose()
        {
            layer.PropertyChanged -= OnLayerPropertyChanged;
            blendEffect.Dispose();
        }

        public ICanvasImage Background
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

        public ICanvasImage Output
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

        public event EventHandler OutputChanged;

        protected void UpdateOutput()
        {
            this.Output = ResolveOutput(background);
        }

        protected virtual void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Layer.Name))
            {
                UpdateOutput();
            }
        }

        protected abstract ICanvasImage ResolveOutput(ICanvasImage background);

        protected ICanvasImage Blend(ICanvasImage foreground, ICanvasImage background)
        {
            blendEffect.Mode = Enum.Parse<BlendEffectMode>(Enum.GetName(typeof(BlendMode), layer.BlendMode));
            blendEffect.Foreground = foreground;
            blendEffect.Background = background;
            return blendEffect;
        }
    }
}