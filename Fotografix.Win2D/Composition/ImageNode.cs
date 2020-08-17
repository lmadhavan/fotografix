using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Fotografix.Win2D.Composition
{
    internal sealed class ImageNode : IDisposable
    {
        private readonly Image image;
        private readonly Dictionary<Layer, LayerNode> layerNodes = new Dictionary<Layer, LayerNode>();

        private ICanvasImage output;
        private bool relinking;

        internal ImageNode(Image image)
        {
            this.image = image;
            RegisterAll();
            RelinkLayers();
            image.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        public void Dispose()
        {
            image.Layers.CollectionChanged -= OnLayerCollectionChanged;
            UnregisterAll();
        }

        public void Draw(CanvasDrawingSession ds)
        {
            if (output != null)
            {
                ds.DrawImage(output);
            }
        }

        public void BeginPreview(Layer layer, IDrawable drawable)
        {
            layerNodes[layer].BeginPreview(drawable);
        }

        public void EndPreview(Layer layer)
        {
            layerNodes[layer].EndPreview();
        }

        private void RelinkLayers()
        {
            if (relinking)
            {
                return;
            }

            this.relinking = true;

            IList<Layer> layers = image.Layers;
            int n = layers.Count;

            if (n == 0)
            {
                this.output = null;
            }
            else
            {

                layerNodes[layers[0]].Background = null;

                for (int i = 1; i < n; i++)
                {
                    layerNodes[layers[i]].Background = layerNodes[layers[i - 1]].Output;
                }

                this.output = layerNodes[layers[n - 1]].Output;
            }

            this.relinking = false;
        }

        private void Register(Layer layer)
        {
            LayerNode node = NodeFactory.Layer.Create(layer);
            node.OutputChanged += OnLayerOutputChanged;
            this.layerNodes[layer] = node;
        }

        private void Unregister(Layer layer)
        {
            if (layerNodes.Remove(layer, out LayerNode node))
            {
                node.OutputChanged -= OnLayerOutputChanged;
                node.Dispose();
            }
        }

        private void RegisterAll()
        {
            foreach (Layer layer in image.Layers)
            {
                Register(layer);
            }
        }

        private void UnregisterAll()
        {
            foreach (LayerNode node in layerNodes.Values)
            {
                node.Dispose();
            }

            layerNodes.Clear();
        }

        private void OnLayerOutputChanged(object sender, EventArgs e)
        {
            RelinkLayers();
        }

        private void OnLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UnregisterAll();
                RegisterAll();
            }
            else if (e.Action != NotifyCollectionChangedAction.Move)
            {
                if (e.OldItems != null)
                {
                    foreach (Layer layer in e.OldItems)
                    {
                        Unregister(layer);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (Layer layer in e.NewItems)
                    {
                        Register(layer);
                    }
                }
            }

            RelinkLayers();
        }
    }
}
