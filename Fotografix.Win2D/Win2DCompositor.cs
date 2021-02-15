using Fotografix.Editor;
using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;
using System.ComponentModel;
using Windows.Foundation;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositor : IDisposable, ICompositionRoot
    {
        private readonly Viewport viewport;
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly ImageNode imageNode;
        private readonly TransparencyGridNode transparencyGrid;
        private readonly CropPreviewNode cropPreviewNode;

        public Win2DCompositor(Image image, Viewport viewport, int transparencyGridSize)
        {
            this.viewport = viewport;
            viewport.PropertyChanged += Viewport_PropertyChanged;

            this.resourceCreator = CanvasDevice.GetSharedDevice();
            this.imageNode = NodeFactory.CreateNode(image, this);
            this.transparencyGrid = transparencyGridSize > 0 ? new TransparencyGridNode(transparencyGridSize, resourceCreator) : null;
            this.cropPreviewNode = new CropPreviewNode(image, resourceCreator, viewport);
        }

        public void Dispose()
        {
            transparencyGrid?.Dispose();
            imageNode.Dispose();
            viewport.PropertyChanged -= Viewport_PropertyChanged;
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            Rect imageBounds = viewport.ImageBounds.ToWindowsRect();
            transparencyGrid?.Draw(ds, imageBounds);
            imageNode.Draw(ds, imageBounds);
            cropPreviewNode.Draw(ds, imageBounds);
        }

        public void Draw(Bitmap bitmap)
        {
            using (Win2DBitmap win2DBitmap = new Win2DBitmap(bitmap, resourceCreator))
            {
                win2DBitmap.Draw(this);
            }
        }

        public Bitmap ToBitmap()
        {
            Win2DBitmap bitmap = new Win2DBitmap(viewport.Size, resourceCreator);
            bitmap.Draw(this);
            return bitmap.Source;
        }

        private void Viewport_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Invalidate();
        }

        ICanvasResourceCreator ICompositionRoot.ResourceCreator => resourceCreator;

        void ICompositionRoot.Invalidate()
        {
            Invalidate();
        }

        private void Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
