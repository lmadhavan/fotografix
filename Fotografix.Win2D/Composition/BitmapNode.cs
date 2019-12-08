using Microsoft.Graphics.Canvas;
using System;
using System.Drawing;
using Windows.Graphics.DirectX;

namespace Fotografix.Win2D.Composition
{
    internal sealed class BitmapNode : CompositionNode, IDisposable
    {
        private readonly CanvasBitmap bitmap;

        internal BitmapNode(Bitmap bitmap)
        {
            this.bitmap = CanvasBitmap.CreateFromBytes(CanvasDevice.GetSharedDevice(),
                                                       bitmap.Pixels,
                                                       bitmap.Size.Width,
                                                       bitmap.Size.Height,
                                                       DirectXPixelFormat.B8G8R8A8UIntNormalized,
                                                       96,
                                                       CanvasAlphaMode.Premultiplied);
            this.Size = bitmap.Size;
            this.Output = this.bitmap;
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }

        public Size Size { get; }
    }
}
