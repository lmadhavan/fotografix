using System;
using System.Drawing;
using Fotografix.Drawing;
using Fotografix.Editor;
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
        private DrawableNode previewNode;

        protected LayerNode(Layer layer, ICompositionRoot root)
        {
            this.layer = layer;
            this.Root = root;

            this.blendEffect = new BlendEffect();
            layer.PropertyChanged += Layer_PropertyChanged;
            layer.UserPropertyChanged += Layer_UserPropertyChanged;
        }

        public virtual void Dispose()
        {
            layer.UserPropertyChanged -= Layer_UserPropertyChanged;
            layer.PropertyChanged -= Layer_PropertyChanged;
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

        protected abstract Rectangle Bounds { get; }

        protected ICompositionRoot Root { get; }

        protected void UpdateOutput()
        {
            ICanvasImage output = ResolveOutput(background);
            
            if (previewNode != null)
            {
                output = previewNode.ResolveOutput(output);
            }
            
            this.Output = output;
        }

        private void UpdatePreview()
        {
            IDrawable preview = layer.GetDrawingPreview();

            if (preview != null)
            {
                this.previewNode = new DrawableNode(preview, Bounds, Root.ResourceCreator);
                previewNode.OutputChanged += Preview_OutputChanged;
            }
            else
            {
                previewNode.Dispose();
                this.previewNode = null;
            }

            UpdateOutput();
        }

        private void Layer_PropertyChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }

        private void Layer_UserPropertyChanged(object sender, UserPropertyChangedEventArgs e)
        {
            if (e.Key == EditorProperties.DrawingPreviewProperty)
            {
                UpdatePreview();
            }
        }

        private void Preview_OutputChanged(object sender, EventArgs e)
        {
            UpdateOutput();
            Root.Invalidate();
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