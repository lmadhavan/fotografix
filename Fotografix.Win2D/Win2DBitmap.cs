using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using System.Drawing;
using Windows.Graphics.DirectX;

namespace Fotografix.Win2D
{
    public sealed class Win2DBitmap : Bitmap
    {
        private readonly CanvasRenderTarget renderTarget;

        public Win2DBitmap(Size size, ICanvasResourceCreator resourceCreator) : base(size)
        {
            this.renderTarget = new CanvasRenderTarget(resourceCreator,
                                                       size.Width,
                                                       size.Height,
                                                       96,
                                                       DirectXPixelFormat.B8G8R8A8UIntNormalized,
                                                       CanvasAlphaMode.Premultiplied);
        }

        public override void Dispose()
        {
            renderTarget.Dispose();
            base.Dispose();
        }

        internal ICanvasImage Output => renderTarget;

        public override byte[] GetPixelBytes()
        {
            return renderTarget.GetPixelBytes();
        }

        public override void SetPixelBytes(byte[] pixels)
        {
            renderTarget.SetPixelBytes(pixels);
        }

        public Bitmap Scale(Size newSize)
        {
            Win2DBitmap result = new Win2DBitmap(newSize, resourceCreator: renderTarget);

            using (CanvasDrawingSession ds = result.renderTarget.CreateDrawingSession())
            {
                ds.DrawImage(renderTarget,
                             new Windows.Foundation.Rect(0, 0, newSize.Width, newSize.Height),
                             new Windows.Foundation.Rect(0, 0, Size.Width, Size.Height));
            }

            return result;
        }

        internal void Draw(Win2DCompositor compositor)
        {
            using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
            {
                compositor.Draw(ds);
            }
        }

        internal IDrawingContext CreateDrawingContext()
        {
            return new Win2DDrawingContext(renderTarget.CreateDrawingSession(), new Rectangle(Point.Empty, Size));
        }
    }
}
