using Microsoft.Graphics.Canvas;
using System;
using System.Drawing;
using Windows.Graphics.DirectX;

namespace Fotografix.Win2D
{
    internal sealed class Win2DBitmap : IDisposable
    {
        private readonly CanvasRenderTarget renderTarget;
        private bool updating;

        public Win2DBitmap(Size size, ICanvasResourceCreator resourceCreator) : this(new Bitmap(size), resourceCreator)
        {
        }

        public Win2DBitmap(Bitmap source, ICanvasResourceCreator resourceCreator)
        {
            this.Source = source;
            Source.ContentChanged += OnSourceContentChanged;

            this.renderTarget = new CanvasRenderTarget(resourceCreator,
                                                       Size.Width,
                                                       Size.Height,
                                                       96,
                                                       DirectXPixelFormat.B8G8R8A8UIntNormalized,
                                                       CanvasAlphaMode.Premultiplied);

            if (source.Size != Size.Empty)
            {
                renderTarget.SetPixelBytes(source.Pixels);
            }
        }

        public void Dispose()
        {
            Source.ContentChanged -= OnSourceContentChanged;
            renderTarget.Dispose();
        }

        public Size Size => Source.Size;
        public Bitmap Source { get; }
        internal ICanvasImage Output => renderTarget;

        public Win2DBitmap Scale(Size newSize)
        {
            Win2DBitmap result = new Win2DBitmap(newSize, resourceCreator: renderTarget);

            using (CanvasDrawingSession ds = result.renderTarget.CreateDrawingSession())
            {
                ds.DrawImage(renderTarget,
                             new Windows.Foundation.Rect(0, 0, newSize.Width, newSize.Height),
                             new Windows.Foundation.Rect(0, 0, Size.Width, Size.Height));
            }

            result.UpdateSource();
            return result;
        }

        public void Draw(Win2DCompositor compositor)
        {
            using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
            {
                compositor.Draw(ds);
            }

            UpdateSource();
        }

        internal Win2DDrawingContext CreateDrawingContext()
        {
            return new Win2DDrawingContext(renderTarget.CreateDrawingSession(), new Rectangle(Point.Empty, Source.Size));
        }

        public void UpdateSource()
        {
            this.updating = true;
            Source.Pixels = renderTarget.GetPixelBytes();
            this.updating = false;
        }

        private void OnSourceContentChanged(object sender, ContentChangedEventArgs e)
        {
            if (!updating)
            {
                renderTarget.SetPixelBytes(Source.Pixels);
            }
        }
    }
}
