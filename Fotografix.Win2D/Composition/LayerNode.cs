using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition
{
    internal sealed class LayerNode : IDisposable
    {
        private readonly Layer layer;
        private readonly NodeFactory nodeFactory;

        private IBlendingStrategy blendingStrategy;
        private ICanvasImage background;
        private ICanvasImage output;

        internal LayerNode(Layer layer, NodeFactory nodeFactory)
        {
            this.layer = layer;
            this.nodeFactory = nodeFactory;

            UpdateBlendingStrategy();
            layer.PropertyChanged += Layer_PropertyChanged;
        }

        public void Dispose()
        {
            layer.PropertyChanged -= Layer_PropertyChanged;
            blendingStrategy.Dispose();
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

        private void UpdateBlendingStrategy()
        {
            blendingStrategy?.Dispose();
            this.blendingStrategy = nodeFactory.CreateBlendingStrategy(layer.Content);
            blendingStrategy.Invalidated += Content_Invalidated;
            UpdateOutput();
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

        private void Content_Invalidated(object sender, EventArgs e)
        {
            UpdateOutput();
            nodeFactory.Invalidate();
        }
    }
}