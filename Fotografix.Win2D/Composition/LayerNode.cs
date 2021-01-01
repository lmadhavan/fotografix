using System;
using System.Drawing;
using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition
{
    internal abstract class LayerNode : IDisposable
    {
        private readonly Layer layer;
        private readonly ICompositionRoot root;
        private readonly BlendEffect blendEffect;

        private ICanvasImage background;
        private ICanvasImage output;
        private DrawableNode drawableNode;

        protected LayerNode(Layer layer, ICompositionRoot root)
        {
            this.layer = layer;
            this.root = root;

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

        public void BeginPreview(IDrawable drawable)
        {
            this.drawableNode = new DrawableNode(drawable, Bounds);
            drawableNode.OutputChanged += OnPreviewContentChanged;
            UpdateOutput();
        }

        public void EndPreview()
        {
            drawableNode.Dispose();
            this.drawableNode = null;
            UpdateOutput();
        }

        protected abstract Rectangle Bounds { get; }

        protected void UpdateOutput()
        {
            ICanvasImage output = ResolveOutput(background);
            
            if (drawableNode != null)
            {
                output = drawableNode.ResolveOutput(output);
            }
            
            this.Output = output;
        }

        private void OnLayerPropertyChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }

        private void OnPreviewContentChanged(object sender, EventArgs e)
        {
            UpdateOutput();
            root.Invalidate();
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