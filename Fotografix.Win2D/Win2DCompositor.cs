using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System;

namespace Fotografix.Win2D
{
    public sealed class Win2DCompositor : IDisposable
    {
        private readonly ImageNode imageNode;

        public Win2DCompositor(Image image)
        {
            this.imageNode = new ImageNode(image);
        }

        public void Dispose()
        {
            imageNode.Dispose();
        }

        public event EventHandler Invalidated
        {
            add => imageNode.Invalidated += value;
            remove => imageNode.Invalidated -= value;
        }

        public void Draw(CanvasDrawingSession ds)
        {
            ICanvasImage output = imageNode.Output;

            if (output != null)
            {
                ds.DrawImage(output);
            }
        }
    }
}
