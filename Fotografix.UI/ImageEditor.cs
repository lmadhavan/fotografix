using Fotografix.Composition;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.UI
{
    public sealed class ImageEditor : NotifyPropertyChangedBase, IDisposable
    {
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly Image image;
        private readonly ReversedCollectionView<Layer> layers;

        private Layer activeLayer;
        private BlendModeListItem selectedBlendMode;

        public ImageEditor(ICanvasResourceCreator resourceCreator, int width, int height)
            : this(resourceCreator, new BitmapLayer(resourceCreator, width, height))
        {
        }

        private ImageEditor(ICanvasResourceCreator resourceCreator, BitmapLayer layer)
        {
            this.resourceCreator = resourceCreator;
            this.image = new Image(layer);
            this.layers = new ReversedCollectionView<Layer>(image.Layers);
            this.activeLayer = layer;

            image.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        public static async Task<ImageEditor> CreateAsync(ICanvasResourceCreator resourceCreator, StorageFile file)
        {
            BitmapLayer layer = await BitmapLayer.LoadAsync(resourceCreator, file);
            return new ImageEditor(resourceCreator, layer);
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

        public Layer ActiveLayer
        {
            get
            {
                return activeLayer;
            }

            set
            {
                if (SetValue(ref activeLayer, value))
                {
                    if (activeLayer != null)
                    {
                        SelectedBlendMode = BlendModes[activeLayer.BlendMode];
                    }

                    RaisePropertyChanged(nameof(CanDeleteActiveLayer));
                    RaisePropertyChanged(nameof(IsBlendModeEnabled));
                }
            }
        }

        public bool IsBlendModeEnabled => activeLayer != image.Layers[0];

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
                    if (activeLayer != null)
                    {
                        activeLayer.BlendMode = selectedBlendMode.BlendMode;
                    }
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
        }

        public bool CanDeleteActiveLayer => activeLayer != image.Layers[0];

        public void DeleteActiveLayer()
        {
            Layer layer = activeLayer;
            image.Layers.Remove(layer);
            layer.Dispose();
        }

        public async Task ImportAsync(IReadOnlyList<StorageFile> files)
        {
            foreach (var file in files)
            {
                BitmapLayer layer = await BitmapLayer.LoadAsync(resourceCreator, file);
                image.Layers.Add(layer);
            }
        }

        private void OnLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Move:
                    this.ActiveLayer = image.Layers[e.NewStartingIndex];
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (ActiveLayer == e.OldItems[0])
                    {
                        int previousIndex = Math.Max(0, e.OldStartingIndex - 1);
                        this.ActiveLayer = image.Layers[previousIndex];
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (ActiveLayer == e.OldItems[0])
                    {
                        this.ActiveLayer = (Layer)e.NewItems[0];
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.ActiveLayer = null;
                    break;
            }
        }
    }
}
