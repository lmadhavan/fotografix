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
        private readonly ObservableCollection<Layer> layers;
        private ICanvasImage output;

        public Image(ICanvasResourceCreator resourceCreator, int width, int height)
            : this(new CanvasRenderTarget(resourceCreator, width, height, 96))
        {
        }

        public Image(CanvasBitmap bitmap)
        {
            this.size = bitmap.SizeInPixels;
            this.layers = new ObservableCollection<Layer>();
            this.Layers = new ReadOnlyObservableCollection<Layer>(layers);
            AddLayer(new BitmapLayer(bitmap));
        }

        public void Dispose()
        {
            foreach (Layer layer in layers)
            {
                layer.Dispose();
            }
        }

        public event EventHandler Invalidated;

        public int Width => (int)size.Width;
        public int Height => (int)size.Height;

        public ReadOnlyObservableCollection<Layer> Layers { get; }

        public static async Task<Image> LoadAsync(StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                var bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
                var image = new Image(bitmap);
                image.layers[0].Name = file.DisplayName;
                return image;
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            drawingSession.DrawImage(output);
        }

        public void AddLayer(Layer layer)
        {
            layer.Invalidated += OnLayerInvalidated;
            layer.OutputChanged += OnLayerOutputChanged;

            layers.Add(layer);
            RelinkLayers();
        }

        public void DeleteLayer(Layer layer)
        {
            if (layers.Count == 1)
            {
                throw new InvalidOperationException("Image must contain at least one layer");
            }

            if (layers.Remove(layer))
            {
                layer.Invalidated -= OnLayerInvalidated;
                layer.OutputChanged -= OnLayerOutputChanged;

                RelinkLayers();
            }
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

            int n = layers.Count;

            layers[0].Background = null;

            for (int i = 1; i < n; i++)
            {
                layers[i].Background = layers[i - 1].Output;
            }

            this.output = layers[n - 1].Output;

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
    }
}