﻿using Fotografix.Editor.Commands;
using Fotografix.UI.Adjustments;
using Fotografix.UI.Collections;
using Fotografix.UI.Layers;
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
    public sealed class ImageEditor : NotifyPropertyChangedBase, ICommandService, IDisposable
    {
        private readonly Image image;
        private readonly Win2DCompositor compositor;
        private readonly ReversedCollectionView<Layer> layers;
        private readonly History history;

        private Layer activeLayer;
        private LayerViewModel activeLayerViewModel;
        
        private ImageEditor(Image image)
        {
            this.image = image;
            this.compositor = new Win2DCompositor(image);

            ReorderAwareCollectionView<Layer> reorderAwareCollectionView = new ReorderAwareCollectionView<Layer>(image.Layers);
            reorderAwareCollectionView.ItemReordered += OnLayerReordered;
            this.layers = new ReversedCollectionView<Layer>(reorderAwareCollectionView);

            this.history = new History();
            history.PropertyChanged += OnHistoryPropertyChanged;

            this.ActiveLayer = image.Layers.FirstOrDefault();

            image.PropertyChanged += OnImagePropertyChanged;
            image.Layers.CollectionChanged += OnLayerCollectionChanged;
        }

        public static ImageEditor Create(Size size)
        {
            BitmapLayer layer = BitmapLayerFactory.CreateBitmapLayer(1);

            Image image = new Image(size);
            image.Layers.Add(layer);

            return new ImageEditor(image);
        }

        public static async Task<ImageEditor> CreateAsync(StorageFile file)
        {
            BitmapLayer layer = await BitmapLayerFactory.LoadBitmapLayerAsync(file);

            Image image = new Image(layer.Bitmap.Size);
            image.Layers.Add(layer);

            return new ImageEditor(image);
        }

        public void Dispose()
        {
            activeLayerViewModel?.Dispose();
            layers.Dispose();
            compositor.Dispose();
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
                if (SetProperty(ref activeLayer, value))
                {
                    activeLayerViewModel?.Dispose();
                    this.ActiveLayerViewModel = activeLayer == null ? null : new LayerViewModel(activeLayer, this);
                    RaisePropertyChanged(nameof(CanDeleteActiveLayer));
                }
            }
        }

        public LayerViewModel ActiveLayerViewModel
        {
            get => activeLayerViewModel;
            private set => SetProperty(ref activeLayerViewModel, value);
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
            List<ICommand> commands = new List<ICommand>();

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

                case NotifyCollectionChangedAction.Reset:
                    this.ActiveLayer = null;
                    break;
            }
        }

        public void Execute(ICommand command)
        {
            IChange change = command.PrepareChange();

            if (change != null)
            {
                change.Apply();
                history.Add(change);
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
    }
}