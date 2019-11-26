using Fotografix.UI.Adjustments;
using Fotografix.UI.BlendModes;
using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.UI
{
    public sealed class ImageEditor : NotifyPropertyChangedBase, IDisposable
    {
        private readonly Image image;
        private readonly Win2DCompositor compositor;
        private readonly ReversedCollectionView<Layer> layers;

        private Layer activeLayer;
        private BlendModeListItem selectedBlendMode;

        private ImageEditor(Image image)
        {
            this.image = image;
            this.compositor = new Win2DCompositor(image);

            this.layers = new ReversedCollectionView<Layer>(image.Layers);
            this.activeLayer = image.Layers.FirstOrDefault();

            image.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        public static ImageEditor Create(Size size, ICanvasResourceCreator resourceCreator)
        {
            BitmapLayer layer = CreateLayer(1);

            Image image = new Image(size);
            image.Layers.Add(layer);

            return new ImageEditor(image);
        }

        public static async Task<ImageEditor> CreateAsync(StorageFile file, ICanvasResourceCreator resourceCreator)
        {
            BitmapLayer layer = await BitmapLayerFactory.LoadBitmapLayerAsync(file);

            Image image = new Image(layer.Bitmap.Size);
            image.Layers.Add(layer);

            return new ImageEditor(image);
        }

        public void Dispose()
        {
            layers.Dispose();
            compositor.Dispose();
        }

        public Size Size => image.Size;

        public IList<Layer> Layers => layers;

        public Layer ActiveLayer
        {
            get
            {
                return activeLayer;
            }

            set
            {
                if (SetProperty(ref activeLayer, value))
                {
                    if (activeLayer != null)
                    {
                        SelectedBlendMode = BlendModes[activeLayer.BlendMode];
                    }

                    RaisePropertyChanged(nameof(CanDeleteActiveLayer));
                }
            }
        }

        public BlendModeList BlendModes { get; } = BlendModeList.Create();

        public BlendModeListItem SelectedBlendMode
        {
            get
            {
                return selectedBlendMode;
            }

            set
            {
                if (SetProperty(ref selectedBlendMode, value))
                {
                    if (activeLayer != null)
                    {
                        activeLayer.BlendMode = selectedBlendMode.BlendMode;
                    }
                }
            }
        }

        public event EventHandler Invalidated
        {
            add => compositor.Invalidated += value;
            remove => compositor.Invalidated -= value;
        }

        public void Draw(CanvasDrawingSession ds)
        {
            compositor.Draw(ds);
        }

        public void AddLayer()
        {
            image.Layers.Add(CreateLayer(image.Layers.Count + 1));
        }

        public void AddAdjustmentLayer(IAdjustmentLayerFactory adjustmentLayerFactory)
        {
            image.Layers.Add(adjustmentLayerFactory.CreateAdjustmentLayer());
        }

        public bool CanDeleteActiveLayer => activeLayer != image.Layers[0];

        public void DeleteActiveLayer()
        {
            Layer layer = activeLayer;
            image.Layers.Remove(layer);
        }

        public async Task ImportLayersAsync(IEnumerable<StorageFile> files)
        {
            foreach (var file in files)
            {
                BitmapLayer layer = await BitmapLayerFactory.LoadBitmapLayerAsync(file);
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

        private static BitmapLayer CreateLayer(int id)
        {
            return new BitmapLayer(Bitmap.Empty) { Name = "Layer " + id };
        }
    }
}
