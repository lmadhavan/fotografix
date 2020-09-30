using Fotografix.Adjustments;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Fotografix.Editor
{
    public sealed class ImageChangeTracker : NotifyContentChangedBase, IDisposable
    {
        private readonly Image image;
        private readonly List<Layer> trackedLayers;

        public ImageChangeTracker(Image image)
        {
            this.image = image;
            image.PropertyChanged += OnImagePropertyChanged;
            image.Layers.CollectionChanged += OnLayerCollectionChanged;

            this.trackedLayers = new List<Layer>(image.Layers);
            foreach (Layer layer in image.Layers)
            {
                Register(layer);
            }
        }

        public void Dispose()
        {
            UnregisterAllLayers();
            image.Layers.CollectionChanged -= OnLayerCollectionChanged;
            image.PropertyChanged -= OnImagePropertyChanged;
        }

        private void Register(Layer layer)
        {
            layer.PropertyChanged += OnLayerPropertyChanged;
            layer.Accept(new ContentRegisteringVisitor(this));
        }

        private void Unregister(Layer layer)
        {
            layer.PropertyChanged -= OnLayerPropertyChanged;
            layer.Accept(new ContentUnregisteringVisitor(this));
        }

        private void UnregisterAllLayers()
        {
            foreach (Layer layer in trackedLayers)
            {
                Unregister(layer);
            }

            trackedLayers.Clear();
        }

        private void Register(Bitmap bitmap)
        {
            bitmap.ContentChanged += OnBitmapContentChanged;
        }

        private void Unregister(Bitmap bitmap)
        {
            bitmap.ContentChanged -= OnBitmapContentChanged;
        }

        private void Register(Adjustment adjustment)
        {
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;
        }

        private void Unregister(Adjustment adjustment)
        {
            adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
        }

        private void OnImagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseContentChanged();
        }

        private void OnLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Layer layer in e.OldItems)
                {
                    Unregister(layer);
                    trackedLayers.Remove(layer);
                }
            }

            if (e.NewItems != null)
            {
                foreach (Layer layer in e.NewItems)
                {
                    trackedLayers.Add(layer);
                    Register(layer);
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UnregisterAllLayers();
            }

            RaiseContentChanged();
        }

        private void OnLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseContentChanged();
        }

        private void OnBitmapContentChanged(object sender, ContentChangedEventArgs e)
        {
            RaiseContentChanged();
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseContentChanged();
        }

        private class ContentRegisteringVisitor : LayerVisitor
        {
            private readonly ImageChangeTracker tracker;

            public ContentRegisteringVisitor(ImageChangeTracker tracker)
            {
                this.tracker = tracker;
            }

            public override void Visit(BitmapLayer layer)
            {
                tracker.Register(layer.Bitmap);
            }

            public override void Visit(AdjustmentLayer layer)
            {
                tracker.Register(layer.Adjustment);
            }
        }

        private class ContentUnregisteringVisitor : LayerVisitor
        {
            private readonly ImageChangeTracker tracker;

            public ContentUnregisteringVisitor(ImageChangeTracker tracker)
            {
                this.tracker = tracker;
            }

            public override void Visit(BitmapLayer layer)
            {
                tracker.Unregister(layer.Bitmap);
            }

            public override void Visit(AdjustmentLayer layer)
            {
                tracker.Unregister(layer.Adjustment);
            }
        }
    }
}
