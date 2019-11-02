using Microsoft.Graphics.Canvas;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Fotografix.Composition
{
    public sealed class Image : IDisposable
    {
        private readonly BitmapSize size;
        private ICanvasImage output;

        public Image(ICanvasResourceCreator resourceCreator, int width, int height)
            : this(new CanvasRenderTarget(resourceCreator, width, height, 96))
        {
        }

        public Image(CanvasBitmap bitmap)
        {
            this.size = bitmap.SizeInPixels;
            this.Layers = new LayerList(this);
            Layers.Add(new BitmapLayer(bitmap));
        }

        public void Dispose()
        {
            foreach (Layer layer in Layers)
            {
                layer.Dispose();
            }
        }

        public event EventHandler Invalidated;

        public int Width => (int)size.Width;
        public int Height => (int)size.Height;

        public ObservableCollection<Layer> Layers { get; }

        public static async Task<Image> LoadAsync(StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                var bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
                var image = new Image(bitmap);
                image.Layers[0].Name = file.DisplayName;
                return image;
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            drawingSession.DrawImage(output);
        }

        private void OnLayerInvalidated(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnLayerOutputChanged(object sender, EventArgs e)
        {
            RelinkLayers();
        }

        private bool relinking;

        private void RelinkLayers()
        {
            if (relinking)
            {
                return;
            }

            this.relinking = true;

            int n = Layers.Count;

            Layers[0].Background = null;

            for (int i = 1; i < n; i++)
            {
                Layers[i].Background = Layers[i - 1].Output;
            }

            this.output = Layers[n - 1].Output;

            this.relinking = false;
            Invalidate();
        }

        private void Invalidate()
        {
            if (relinking)
            {
                return;
            }

            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        private void Register(Layer layer)
        {
            layer.Invalidated += OnLayerInvalidated;
            layer.OutputChanged += OnLayerOutputChanged;
            
            RelinkLayers();
        }

        private void Unregister(Layer layer, bool relink = true)
        {
            layer.Invalidated -= OnLayerInvalidated;
            layer.OutputChanged -= OnLayerOutputChanged;
            
            if (relink)
            {
                RelinkLayers();
            }
        }

        private class LayerList : ObservableCollection<Layer>
        {
            private readonly Image image;

            internal LayerList(Image image)
            {
                this.image = image;
            }

            protected override void ClearItems()
            {
                foreach (Layer layer in this)
                {
                    image.Unregister(layer, false);
                }

                base.ClearItems();
                image.RelinkLayers();
            }

            protected override void InsertItem(int index, Layer layer)
            {
                base.InsertItem(index, layer);
                image.Register(layer);
            }

            protected override void MoveItem(int oldIndex, int newIndex)
            {
                base.MoveItem(oldIndex, newIndex);
                image.RelinkLayers();
            }

            protected override void RemoveItem(int index)
            {
                Layer layer = this[index];
                base.RemoveItem(index);
                image.Unregister(layer);
            }

            protected override void SetItem(int index, Layer layer)
            {
                image.Unregister(this[index], false);
                base.SetItem(index, layer);
                image.Register(layer);
            }
        }
    }
}