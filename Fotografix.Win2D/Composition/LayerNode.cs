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
        private IComposableNode drawingPreviewNode;

        protected LayerNode(Layer layer, NodeFactory nodeFactory)
        {
            this.layer = layer;
            this.NodeFactory = nodeFactory;

            this.blendEffect = new BlendEffect();
            layer.PropertyChanged += Layer_PropertyChanged;
            layer.UserPropertyChanged += Layer_PropertyChanged;

            this.drawingPreviewNode = new NullComposableNode();
        }

        public virtual void Dispose()
        {
            drawingPreviewNode.Dispose();

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

        protected NodeFactory NodeFactory { get; }

        protected void UpdateOutput()
        {
            this.Output = drawingPreviewNode.Compose(ResolveOutput(background));
        }

        private void UpdatePreview()
        {
            IDrawable drawable = layer.GetDrawingPreview();
            drawingPreviewNode?.Dispose();

            if (drawable != null)
            {
                this.drawingPreviewNode = NodeFactory.CreateDrawingPreviewNode(drawable, Bounds);
                drawingPreviewNode.Invalidated += Preview_Invalidated;
            }
            else
            {
                this.drawingPreviewNode = new NullComposableNode();
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

        private void Preview_Invalidated(object sender, EventArgs e)
        {
            UpdateOutput();
            NodeFactory.Invalidate();
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