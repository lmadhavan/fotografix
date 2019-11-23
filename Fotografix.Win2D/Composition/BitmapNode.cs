using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BitmapNode : CompositionNode, IBitmap
    {
        private readonly CanvasBitmap bitmap;

        internal BitmapNode(CanvasBitmap bitmap)
        {
            this.bitmap = bitmap;
            this.Size = new Size((int)bitmap.SizeInPixels.Width, (int)bitmap.SizeInPixels.Height);
            this.Output = bitmap;
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }

        public Size Size { get; }
    }
}
