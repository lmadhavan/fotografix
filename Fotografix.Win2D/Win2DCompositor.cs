using Fotografix.Drawing;
using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositor : IDisposable, ICompositionRoot
    {
        private readonly Image image;
        private readonly ICanvasResourceCreator resourceCreator;
        private readonly ImageNode imageNode;
        private readonly TransparencyGridNode transparencyGrid;

        public Win2DCompositor(Image image) : this(image, 0)
        {
        }

        public Win2DCompositor(Image image, int transparencyGridSize)
        {
            this.image = image;
            this.resourceCreator = CanvasDevice.GetSharedDevice();
            this.imageNode = NodeFactory.CreateNode(image, this);
            this.transparencyGrid = transparencyGridSize > 0 ? new TransparencyGridNode(transparencyGridSize, resourceCreator) : null;
        }

        public void Dispose()
        {
            transparencyGrid?.Dispose();
            imageNode.Dispose();
        }

        public event EventHandler Invalidated;

        public void Draw(CanvasDrawingSession ds)
        {
            transparencyGrid?.Draw(ds);
            imageNode.Draw(ds);
        }

        public void BeginPreview(Layer layer, IDrawable drawable)
        {
            imageNode.BeginPreview(layer, drawable);
        }

        public void EndPreview(Layer layer)
        {
            imageNode.EndPreview(layer);
        }

        public Bitmap ToBitmap()
        {
            Win2DBitmap bitmap = new Win2DBitmap(image.Size, resourceCreator);
            bitmap.Draw(this);
            return bitmap.Source;
        }

        ICanvasResourceCreator ICompositionRoot.ResourceCreator => resourceCreator;

        void ICompositionRoot.Invalidate()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }
    }
}
