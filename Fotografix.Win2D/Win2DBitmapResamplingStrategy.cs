using Fotografix.Editor;
using Fotografix.Win2D.Composition;
using Microsoft.Graphics.Canvas;
using System.Drawing;
using Windows.Graphics.DirectX;

namespace Fotografix.Win2D
{
    public sealed class Win2DBitmapResamplingStrategy : IBitmapResamplingStrategy
    {
        public Bitmap Resample(Bitmap bitmap, Size newSize)
        {
            if (newSize.IsEmpty)
            {
                return Bitmap.Empty;
            }

            using (BitmapNode node = new BitmapNode(bitmap))
            using (CanvasRenderTarget renderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(),
                                                                            newSize.Width,
                                                                            newSize.Height,
                                                                            96,
                                                                            DirectXPixelFormat.B8G8R8A8UIntNormalized,
                                                                            CanvasAlphaMode.Premultiplied))
            {
                using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
                {
                    ds.DrawImage(node.Output,
                                 new Windows.Foundation.Rect(0, 0, newSize.Width, newSize.Height),
                                 new Windows.Foundation.Rect(0, 0, node.Size.Width, node.Size.Height));
                }

                return new Bitmap(newSize, renderTarget.GetPixelBytes());
            }
        }
    }
}
