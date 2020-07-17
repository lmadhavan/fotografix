using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositor : IDisposable
    {
        private readonly Image image;
        private readonly ImageNode imageNode;
        private readonly TransparencyGridNode transparencyGrid;

        public Win2DCompositor(Image image) : this(image, 0)
        {
        }

        public Win2DCompositor(Image image, int transparencyGridSize)
        {
            this.image = image;
            this.imageNode = new ImageNode(image);
            this.transparencyGrid = transparencyGridSize > 0 ? new TransparencyGridNode(transparencyGridSize) : null;
        }

        public void Dispose()
        {
            transparencyGrid?.Dispose();
            imageNode.Dispose();
        }

        public void Draw(CanvasDrawingSession ds)
        {
            transparencyGrid?.Draw(ds);
            imageNode.Draw(ds);
        }

        public void BeginBrushStrokePreview(Layer layer, BrushStroke brushStroke)
        {
            imageNode.BeginBrushStrokePreview(layer, brushStroke);
        }

        public void EndBrushStrokePreview(Layer layer)
        {
            imageNode.EndBrushStrokePreview(layer);
        }

        public Bitmap ToBitmap()
        {
            Win2DBitmap bitmap = new Win2DBitmap(image.Size, CanvasDevice.GetSharedDevice());
            bitmap.Draw(this);
            return bitmap;
        }
    }
}
