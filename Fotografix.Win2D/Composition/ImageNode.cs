using Fotografix.Editor;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;

namespace Fotografix.Win2D.Composition
{
    internal sealed class ImageNode : IDisposable
    {
        private readonly Image image;
        private readonly ICompositionRoot root;
        private readonly Dictionary<Layer, LayerNode> layerNodes = new Dictionary<Layer, LayerNode>();

        private ICanvasImage output;
        private CropPreviewNode cropPreviewNode;
        private bool relinking;

        internal ImageNode(Image image, ICompositionRoot root)
        {
            this.image = image;
            this.root = root;

            RegisterAll();
            RelinkLayers();

            image.ContentChanged += Image_ContentChanged;
            image.UserPropertyChanged += Image_UserPropertyChanged;
            image.Layers.CollectionChanged += Layers_CollectionChanged;
        }

        public void Dispose()
        {
            cropPreviewNode?.Dispose();

            image.Layers.CollectionChanged -= Layers_CollectionChanged;
            image.UserPropertyChanged -= Image_UserPropertyChanged;
            image.ContentChanged -= Image_ContentChanged;
            UnregisterAll();
        }

        public void Draw(CanvasDrawingSession ds)
        {
            if (output != null)
            {
                ds.DrawImage(output);
            }

            cropPreviewNode?.Draw(ds);
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
            LayerNode node = NodeFactory.CreateNode(layer, root);
            node.OutputChanged += Layer_OutputChanged;
            this.layerNodes[layer] = node;
        }

        private void Unregister(Layer layer)
        {
            if (layerNodes.Remove(layer, out LayerNode node))
            {
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

        private void Image_ContentChanged(object sender, ContentChangedEventArgs e)
        {
            root.Invalidate();
        }

        private void Image_UserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == EditorProperties.CropPreview)
            {
                UpdateCropPreview();
            }
        }

        private void UpdateCropPreview()
        {
            Rectangle? rect = image.GetCropPreview();
            cropPreviewNode?.Dispose();

            if (rect == null)
            {
                this.cropPreviewNode = null;
                return;
            }

            this.cropPreviewNode = new CropPreviewNode(root.ResourceCreator, image.Size, rect.Value);
            root.Invalidate();
        }

        private void Layer_OutputChanged(object sender, EventArgs e)
        {
            RelinkLayers();
        }

        private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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
