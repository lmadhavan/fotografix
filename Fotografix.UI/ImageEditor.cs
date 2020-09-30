using Fotografix.Drawing;
using Fotografix.Editor;
using Fotografix.Editor.Collections;
using Fotografix.Editor.Drawing;
using Fotografix.Editor.Layers;
using Fotografix.Editor.PropertyModel;
using Fotografix.Editor.Tools;
using Fotografix.UI.Adjustments;
using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.UI
{
    public sealed class ImageEditor : NotifyPropertyChangedBase, ICommandService, IDisposable, IToolbox
    {
        private static readonly IDrawingContextFactory DrawingContextFactory = new Win2DDrawingContextFactory();

        private readonly Image image;
        private readonly ImageChangeTracker contentChangeTracker;

        private readonly Win2DCompositor compositor;
        private readonly ReversedCollectionView<Layer> layers;
        private readonly History history;
        private readonly IPropertySetter propertySetter;

        private Layer activeLayer;
        private LayerPropertyEditor activeLayerViewModel;
        private List<IDrawingSurfaceListener> drawingSurfaceListeners;

        private ImageEditor(Image image, Viewport viewport)
        {
            this.image = image;

            this.compositor = new Win2DCompositor(image, 8);

            ReorderAwareCollectionView<Layer> reorderAwareCollectionView = new ReorderAwareCollectionView<Layer>(image.Layers);
            reorderAwareCollectionView.ItemReordered += OnLayerReordered;
            this.layers = new ReversedCollectionView<Layer>(reorderAwareCollectionView);

            this.history = new History();
            history.PropertyChanged += OnHistoryPropertyChanged;
            this.propertySetter = new UndoablePropertySetter(history);

            InitializeTools(viewport);

            this.ActiveLayer = image.Layers.First();

            this.contentChangeTracker = new ImageChangeTracker(image);
            contentChangeTracker.ContentChanged += OnContentChanged;

            image.PropertyChanged += OnImagePropertyChanged;
            image.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        public static ImageEditor Create(Size size, Viewport viewport)
        {
            BitmapLayer layer = BitmapLayerFactory.CreateBitmapLayer(1);

            Image image = new Image(size);
            image.Layers.Add(layer);

            return new ImageEditor(image, viewport);
        }

        public static async Task<ImageEditor> CreateAsync(StorageFile file, Viewport viewport)
        {
            BitmapLayer layer = await BitmapLayerFactory.LoadBitmapLayerAsync(file);
            Image image = new Image(layer);
            return new ImageEditor(image, viewport);
        }

        public void Dispose()
        {
            activeLayerViewModel?.Dispose();
            layers.Dispose();
            compositor.Dispose();
            contentChangeTracker.Dispose();
        }

        #region Undo/Redo

        public bool CanUndo => history.CanUndo;
        public bool CanRedo => history.CanRedo;

        public void Undo()
        {
            history.Undo();
        }

        public void Redo()
        {
            history.Redo();
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
                    activeLayerViewModel?.Dispose();
                    this.ActiveLayerPropertyEditor = activeLayer == null ? null : new LayerPropertyEditor(activeLayer, propertySetter);
                    RaisePropertyChanged(nameof(CanDeleteActiveLayer));
                    NotifyDrawingSurfaceListener();
                }
            }
        }

        public LayerPropertyEditor ActiveLayerPropertyEditor
        {
            get => activeLayerViewModel;
            private set => SetProperty(ref activeLayerViewModel, value);
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            compositor.Draw(ds);
        }

        public void AddLayer()
        {
            Layer layer = BitmapLayerFactory.CreateBitmapLayer(image.Layers.Count + 1);
            Execute(new AddLayerCommand(image, layer));
        }

        public void AddAdjustmentLayer(IAdjustmentLayerFactory adjustmentLayerFactory)
        {
            Layer layer = adjustmentLayerFactory.CreateAdjustmentLayer();
            Execute(new AddLayerCommand(image, layer));
        }

        public bool CanDeleteActiveLayer => activeLayer != image.Layers[0];

        public void DeleteActiveLayer()
        {
            Execute(new RemoveLayerCommand(image, activeLayer));
        }

        public async Task ImportLayersAsync(IEnumerable<StorageFile> files)
        {
            List<Command> commands = new List<Command>();

            foreach (var file in files)
            {
                BitmapLayer layer = await BitmapLayerFactory.LoadBitmapLayerAsync(file);
                commands.Add(new AddLayerCommand(image, layer));
            }

            Execute(new CompositeCommand(commands));
        }

        public ResizeImageParameters CreateResizeImageParameters()
        {
            return new ResizeImageParameters(Size);
        }

        public void ResizeImage(ResizeImageParameters resizeImageParameters)
        {
            Execute(new ResampleImageCommand(image, resizeImageParameters.Size, new Win2DBitmapResamplingStrategy()));
        }

        public async Task SaveAsync(StorageFile file)
        {
            Bitmap bitmap = image.ToBitmap(DrawingContextFactory);
            await BitmapCodec.SaveBitmapAsync(file, bitmap);
        }

        public Bitmap ToBitmap()
        {
            return compositor.ToBitmap();
        }

        #region Tools

        private ITool activeTool;

        public IList<ITool> Tools { get; private set; }

        public ITool ActiveTool
        {
            get => activeTool;
            set => SetProperty(ref activeTool, value);
        }

        private void InitializeTools(Viewport viewport)
        {
            var handTool = new HandTool(viewport);

            var brushTool = new BrushTool()
            {
                Size = 5,
                Color = Color.White
            };

            var gradientTool = new GradientTool()
            {
                StartColor = Color.Black,
                EndColor = Color.White
            };

            this.Tools = new List<ITool> { handTool, brushTool, gradientTool };
            this.activeTool = Tools.First();
            this.drawingSurfaceListeners = new List<IDrawingSurfaceListener> { brushTool, gradientTool };
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
            if (command.IsEffective)
            {
                command.Execute();
                history.Add(command);
            }
        }

        private void OnLayerReordered(object sender, ItemReorderedEventArgs args)
        {
            history.Add(new ReorderLayerCommand(image, args.OldIndex, args.NewIndex));
        }

        private void OnHistoryPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        private void OnContentChanged(object sender, ContentChangedEventArgs e)
        {
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
                editor.Execute(new DrawCommand(bitmapLayer, DrawingContextFactory, drawable));
            }
        }
    }
}
