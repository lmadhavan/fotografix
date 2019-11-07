using Fotografix.Composition;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fotografix.UI
{
    public sealed class ImageEditor : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Image image;
        private readonly ReversedCollectionView<Layer> layers;

        private Layer selectedLayer;
        private BlendModeListItem selectedBlendMode;

        public ImageEditor(Image image)
        {
            this.image = image;
            this.layers = new ReversedCollectionView<Layer>(image.Layers);
            this.selectedLayer = image.Layers[0];
        }

        public void Dispose()
        {
            layers.Dispose();
            image.Dispose();
        }

        public event EventHandler Invalidated
        {
            add { image.Invalidated += value; }
            remove { image.Invalidated -= value; }
        }

        public int Width => image.Width;
        public int Height => image.Height;

        public IList<Layer> Layers => layers;

        public Layer SelectedLayer
        {
            get
            {
                return selectedLayer;
            }

            set
            {
                if (SetValue(ref selectedLayer, value))
                {
                    if (selectedLayer != null)
                    {
                        SelectedBlendMode = BlendModes[selectedLayer.BlendMode];
                    }

                    RaisePropertyChanged(nameof(CanDeleteLayer));
                    RaisePropertyChanged(nameof(IsBlendModeEnabled));
                }
            }
        }

        public bool IsBlendModeEnabled => selectedLayer != image.Layers[0];

        public BlendModeList BlendModes { get; } = BlendModeList.Create();

        public BlendModeListItem SelectedBlendMode
        {
            get
            {
                return selectedBlendMode;
            }

            set
            {
                if (SetValue(ref selectedBlendMode, value))
                {
                    if (selectedLayer != null)
                    {
                        selectedLayer.BlendMode = selectedBlendMode.BlendMode;
                    }
                }
            }
        }

        public int SelectedBlendModeIndex
        {
            get
            {
                return selectedLayer == null ? 0 : (int)selectedLayer.BlendMode;
            }

            set
            {
                if (selectedLayer != null)
                {
                    selectedLayer.BlendMode = (BlendMode)value;
                }
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            image.Draw(drawingSession);
        }

        public void AddLayer(Layer layer)
        {
            image.Layers.Add(layer);
            this.SelectedLayer = layer;
        }

        public bool CanDeleteLayer => selectedLayer != image.Layers[0];

        public void DeleteLayer()
        {
            Layer layer = selectedLayer;
            image.Layers.Remove(layer);
            layer.Dispose();
            this.SelectedLayer = image.Layers.Last();
        }
    }
}
