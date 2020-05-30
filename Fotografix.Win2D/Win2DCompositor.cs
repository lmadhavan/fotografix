using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;
using System.Drawing;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositor : IDisposable, IWin2DDrawable
    {
        private readonly Image image;
        private readonly ImageNode imageNode;
        private readonly TransparencyGridNode transparencyGrid;

        public Win2DCompositor(Image image)
        {
            this.image = image;
            this.imageNode = new ImageNode(image);
            this.transparencyGrid = new TransparencyGridNode(8);
        }

        public void Dispose()
        {
            transparencyGrid.Dispose();
            imageNode.Dispose();
        }

        public Size Size => image.Size;

        public void Draw(CanvasDrawingSession ds)
        {
            transparencyGrid.Draw(ds);
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
    }
}
