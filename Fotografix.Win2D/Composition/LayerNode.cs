using System;
using System.ComponentModel;
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
            layer.UserPropertyChanged += Layer_PropertyChanged;
        }

        public virtual void Dispose()
        {
            previewNode?.Dispose();

            layer.UserPropertyChanged -= Layer_PropertyChanged;
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
            previewNode?.Dispose();

            if (preview != null)
            {
                this.previewNode = new DrawableNode(preview, Bounds, Root.ResourceCreator);
                previewNode.OutputChanged += Preview_OutputChanged;
            }
            else
            {
                this.previewNode = null;
            }
        }

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorProperties.DrawingPreview)
            {
                UpdatePreview();
            }

            UpdateOutput();
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