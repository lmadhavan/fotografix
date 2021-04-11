using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition
{
    internal sealed class LayerNode : IDisposable
    {
        private readonly Layer layer;
        private readonly NodeFactory nodeFactory;

        private readonly IComposableNode drawingPreviewNode;
        private IBlendingStrategy blendingStrategy;
        
        private ICanvasImage background;
        private ICanvasImage output;

        internal LayerNode(Layer layer, NodeFactory nodeFactory)
        {
            this.layer = layer;
            this.nodeFactory = nodeFactory;

            this.drawingPreviewNode = nodeFactory.CreateDrawingPreviewNode(layer);
            drawingPreviewNode.Invalidated += Preview_Invalidated;

            UpdateBlendingStrategy();
            layer.PropertyChanged += Layer_PropertyChanged;
        }

        public void Dispose()
        {
            layer.PropertyChanged -= Layer_PropertyChanged;
            blendingStrategy.Dispose();
            drawingPreviewNode.Dispose();
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

        private void UpdateOutput()
        {
            this.Output = blendingStrategy.Blend(layer, background);
        }

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Layer.Content))
            {
                UpdateBlendingStrategy();
            }
            else
            {
                UpdateOutput();
            }
        }

        private void Preview_Invalidated(object sender, EventArgs e)
        {
            UpdateOutput();
            nodeFactory.Invalidate();
        }

        private void UpdateBlendingStrategy()
        {
            blendingStrategy?.Dispose();
            this.blendingStrategy = nodeFactory.CreateBlendingStrategy(layer.Content, drawingPreviewNode);
            UpdateOutput();
        }
    }
}