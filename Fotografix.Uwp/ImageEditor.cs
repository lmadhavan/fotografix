using Fotografix.Editor;
using Fotografix.Editor.Collections;
using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Fotografix.Uwp
{
    public sealed class ImageEditor : NotifyPropertyChangedBase, IDisposable
    {
        private static readonly Win2DCompositorSettings CompositorSettings = new Win2DCompositorSettings { TransparencyGridSize = 8, InteractiveMode = true };

        private readonly Document document;
        private readonly Image image;
        private readonly Viewport viewport;
        private readonly Win2DCompositor compositor;
        private readonly ReversedCollectionView<Layer> layers;

        public ImageEditor(Document document)
        {
            this.document = document;
            document.PropertyChanged += Document_PropertyChanged;

            this.image = document.Image;
            image.PropertyChanged += Image_PropertyChanged;

            this.viewport = document.Viewport;
            viewport.ZoomToFit = true;
            viewport.PropertyChanged += Viewport_PropertyChanged;

            this.compositor = new Win2DCompositor(image, document.Viewport, CompositorSettings);
            this.layers = new ReversedCollectionView<Layer>(image.Layers);
        }

        public void Dispose()
        {
            layers.Dispose();
            compositor.Dispose();
        }

        public IToolbox Toolbox { get; set; }

        public AsyncCommand ImportLayerCommand { get; set; }

        public string Title
        {
            get
            {
                string prefix = document.IsDirty ? "* " : "";
                string filename = document.File?.Name ?? "Untitled";
                return prefix + filename;
            }
        }

        public Size Size => image.Size;

        public Document Document => document;
        public Viewport Viewport => viewport;

        public string ZoomFactor => viewport.ZoomFactor.ToString("P0");
        public bool ZoomToFit => viewport.ZoomToFit;

        public ReversedCollectionView<Layer> Layers => layers;

        public Layer ActiveLayer
        {
            get
            {
                return document.ActiveLayer;
            }

            set
            {
                if (value != null)
                {
                    document.ActiveLayer = value;
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

        public Bitmap ToBitmap()
        {
            return compositor.ToBitmap();
        }

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Document.ActiveLayer):
                    RaisePropertyChanged(nameof(ActiveLayer));
                    break;

                case nameof(Document.File):
                case nameof(Document.IsDirty):
                    RaisePropertyChanged(nameof(Title));
                    break;
            }
        }

        private void Image_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Image.Size))
            {
                RaisePropertyChanged(nameof(Size));
            }
        }

        private void Viewport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Viewport.ZoomFactor):
                case nameof(Viewport.ZoomToFit):
                    RaisePropertyChanged(e.PropertyName);
                    break;
            }
        }
    }
}
