using Fotografix.Editor;
using Fotografix.Editor.Collections;
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
    public sealed class ImageEditor : NotifyPropertyChangedBase, IDisposable, IToolbox, ICommandDispatcher
    {
        private static readonly Win2DCompositorSettings CompositorSettings = new Win2DCompositorSettings { TransparencyGridSize = 8, InteractiveMode = true };

        private readonly Image image;
        private readonly Viewport viewport;
        private readonly ICommandDispatcher dispatcher;
        private readonly Win2DCompositor compositor;
        private readonly ReversedCollectionView<Layer> layers;
        private readonly History history;

        private Layer activeLayer;

        private List<IChange> changeGroup;
        private bool ignoreChanges;

        public ImageEditor(Image image, ICommandDispatcher dispatcher)
        {
            this.image = image;
            
            this.viewport = image.GetViewport();
            viewport.ImageSize = image.Size;

            this.dispatcher = dispatcher;
            this.compositor = new Win2DCompositor(image, viewport, CompositorSettings);

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

        public float ZoomFactor => viewport.ZoomFactor;

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
                    image.SetActiveLayer(activeLayer);
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
            Dispatch(new ResampleImageCommand(image, resizeImageParameters.Size));
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

        public IList<ITool> Tools
        {
            get => tools;

            set
            {
                if (SetProperty(ref tools, value))
                {
                    this.ActiveTool = Tools.FirstOrDefault();
                }
            }
        }

        public ITool ActiveTool
        {
            get => activeTool;

            set
            {
                ITool oldTool = activeTool;

                if (SetProperty(ref activeTool, value))
                {
                    oldTool?.Deactivated();
                    activeTool.Activated(image);
                }
            }
        }

        #endregion

        private void OnImagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Image.Size))
            {
                viewport.ImageSize = image.Size;
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

        public void Dispatch<T>(T command)
        {
            try
            {
                this.changeGroup = new List<IChange>();
                dispatcher.Dispatch(command);
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
        }
    }
}
