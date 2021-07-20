using Fotografix.Editor;
using Fotografix.Editor.Collections;
using Fotografix.IO;
using Fotografix.Uwp.FileManagement;
using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

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

            this.viewport = image.GetViewport();
            viewport.ImageSize = image.Size;

            this.compositor = new Win2DCompositor(image, viewport, CompositorSettings);
            this.layers = new ReversedCollectionView<Layer>(image.Layers);
        }

        public void Dispose()
        {
            layers.Dispose();
            compositor.Dispose();
        }

        public IToolbox Toolbox { get; set; }
        public FilePickerOverride FilePickerOverride { get; set; }

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

        public float ZoomFactor => viewport.ZoomFactor;

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

        public Task ExecuteAsync(IDocumentCommand command)
        {
            return command.ExecuteAsync(document);
        }

        public async Task ImportLayersAsync(IEnumerable<IFile> files)
        {
            using (FilePickerOverride.OverrideOpenFiles(files))
            {
                await ImportLayerCommand.ExecuteAsync();
            }
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
                viewport.ImageSize = image.Size;
                RaisePropertyChanged(nameof(Size));
            }
        }
    }
}
