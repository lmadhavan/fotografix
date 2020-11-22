using Fotografix.Drawing;
using Fotografix.Editor;
using Fotografix.Editor.Collections;
using Fotografix.Editor.Drawing;
using Fotografix.Editor.Tools;
using Fotografix.IO;
using Fotografix.Uwp.Adjustments;
using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Uwp
{
    public sealed class ImageEditor : NotifyPropertyChangedBase, ICommandService, IDisposable, IToolbox
    {
        private static readonly IDrawingContextFactory DrawingContextFactory = new Win2DDrawingContextFactory();

        private readonly Image image;

        private readonly Win2DCompositor compositor;
        private readonly ReversedCollectionView<Layer> layers;
        private readonly History history;

        private Layer activeLayer;

        private List<Change> changeGroup;
        private bool ignoreChanges;

        public ImageEditor(Image image)
        {
            this.image = image;
            this.compositor = new Win2DCompositor(image, 8);

            this.layers = new ReversedCollectionView<Layer>(image.Layers);

            this.history = new History();
            history.PropertyChanged += OnHistoryPropertyChanged;

            this.Tools = new List<ITool>();
            this.ActiveLayer = image.Layers.First();

            image.ContentChanged += OnContentChanged;
            image.PropertyChanged += OnImagePropertyChanged;
            image.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        public void Dispose()
        {
            layers.Dispose();
            compositor.Dispose();
        }

        public IImageDecoder ImageDecoder { get; set; } = NullImageCodec.Instance;
        public IImageEncoder ImageEncoder { get; set; } = NullImageCodec.Instance;

        public IEnumerable<FileFormat> SupportedImportFormats => ImageDecoder.SupportedFileFormats;
        public IEnumerable<FileFormat> SupportedSaveFormats => ImageEncoder.SupportedFileFormats;

        #region Undo/Redo

        public bool CanUndo => history.CanUndo;
        public bool CanRedo => history.CanRedo;

        public void Undo()
        {
            try
            {
                this.ignoreChanges = true;
                history.Undo();
            }
            finally
            {
                this.ignoreChanges = false;
            }
        }

        public void Redo()
        {
            try
            {
                this.ignoreChanges = true;
                history.Redo();
            }
            finally
            {
                this.ignoreChanges = false;
            }
        }

        #endregion

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
                if (value != null && SetProperty(ref activeLayer, value))
                {
                    RaisePropertyChanged(nameof(CanDeleteActiveLayer));
                    NotifyDrawingSurfaceListener();
                }
            }
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            compositor.Draw(ds);
        }

        public void AddLayer()
        {
            Layer layer = BitmapLayerFactory.CreateBitmapLayer(id: image.Layers.Count + 1);
            image.Layers.Add(layer);
        }

        public void AddAdjustmentLayer(IAdjustmentLayerFactory adjustmentLayerFactory)
        {
            Layer layer = adjustmentLayerFactory.CreateAdjustmentLayer();
            image.Layers.Add(layer);
        }

        public bool CanDeleteActiveLayer => activeLayer != image.Layers[0];

        public void DeleteActiveLayer()
        {
            image.Layers.Remove(activeLayer);
        }

        public async Task ImportLayersAsync(IEnumerable<IFile> files)
        {
            List<Command> commands = new List<Command>();

            foreach (var file in files)
            {
                Image importedImage = await ImageDecoder.ReadImageAsync(file);
                foreach (Layer layer in importedImage.DetachLayers())
                {
                    image.Layers.Add(layer);
                }
            }
        }

        public ResizeImageParameters CreateResizeImageParameters()
        {
            return new ResizeImageParameters(Size);
        }

        public void ResizeImage(ResizeImageParameters resizeImageParameters)
        {
            Execute(new ResampleImageCommand(image, resizeImageParameters.Size, new Win2DBitmapResamplingStrategy()));
        }

        public Task SaveAsync(IFile file)
        {
            return ImageEncoder.WriteImageAsync(image, file);
        }

        public Bitmap ToBitmap()
        {
            return compositor.ToBitmap();
        }

        #region Tools

        private IList<ITool> tools;
        private ITool activeTool;
        private List<IDrawingSurfaceListener> drawingSurfaceListeners;

        public IList<ITool> Tools
        {
            get => tools;

            set
            {
                if (SetProperty(ref tools, value))
                {
                    this.drawingSurfaceListeners = tools.OfType<IDrawingSurfaceListener>().ToList();
                    this.ActiveTool = Tools.FirstOrDefault();
                }
            }
        }

        public ITool ActiveTool
        {
            get => activeTool;

            set
            {
                if (SetProperty(ref activeTool, value))
                {
                    NotifyDrawingSurfaceListener();
                }
            }
        }

        #endregion

        private void OnImagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Image.Size))
            {
                RaisePropertyChanged(nameof(Size));
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
            }
        }

        public void Execute(Command command)
        {
            try
            {
                this.changeGroup = new List<Change>();
                command.Execute();
            }
            finally
            {
                history.Add(new CompositeChange(changeGroup));
                this.changeGroup = null;
            }
        }

        private void OnHistoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void OnContentChanged(object sender, ContentChangedEventArgs e)
        {
            if (e.Change != null && !ignoreChanges)
            {
                if (changeGroup != null)
                {
                    changeGroup.Add(e.Change);
                }
                else
                {
                    history.Add(e.Change);
                }
            }

            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        private void NotifyDrawingSurfaceListener()
        {
            if (activeLayer is BitmapLayer bitmapLayer)
            {
                foreach (IDrawingSurfaceListener listener in drawingSurfaceListeners)
                {
                    listener.DrawingSurfaceActivated(new BitmapDrawingSurface(bitmapLayer, this));
                }
            }
        }

        private sealed class BitmapDrawingSurface : IDrawingSurface
        {
            private readonly BitmapLayer bitmapLayer;
            private readonly ImageEditor editor;

            public BitmapDrawingSurface(BitmapLayer bitmapLayer, ImageEditor editor)
            {
                this.bitmapLayer = bitmapLayer;
                this.editor = editor;
            }

            public void BeginDrawing(IDrawable drawable)
            {
                editor.compositor.BeginPreview(bitmapLayer, drawable);
                drawable.ContentChanged += editor.OnContentChanged;
            }

            public void EndDrawing(IDrawable drawable)
            {
                drawable.ContentChanged -= editor.OnContentChanged;
                editor.compositor.EndPreview(bitmapLayer);
                editor.Execute(new DrawCommand(bitmapLayer.Bitmap, DrawingContextFactory, drawable));
            }
        }
    }
}
