using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Windows.Graphics.DirectX;

namespace Fotografix.Win2D
{
    internal sealed class Win2DBitmap : IDisposable
    {
        private readonly CanvasRenderTarget renderTarget;
        private readonly Transform2DEffect transformEffect;
        private bool updating;

        public Win2DBitmap(Size size, ICanvasResourceCreator resourceCreator) : this(new Bitmap(size), resourceCreator)
        {
        }

        public Win2DBitmap(Bitmap source, ICanvasResourceCreator resourceCreator)
        {
            this.Source = source;
            Source.PropertyChanged += Source_PropertyChanged;

            this.renderTarget = new CanvasRenderTarget(resourceCreator,
                                                       Size.Width,
                                                       Size.Height,
                                                       96,
                                                       DirectXPixelFormat.B8G8R8A8UIntNormalized,
                                                       CanvasAlphaMode.Premultiplied);

            this.transformEffect = new Transform2DEffect() { Source = renderTarget };
            UpdateTransform();

            if (source.Size != Size.Empty)
            {
                renderTarget.SetPixelBytes(source.Pixels);
            }
        }

        public void Dispose()
        {
            Source.PropertyChanged -= Source_PropertyChanged;
            transformEffect.Dispose();
            renderTarget.Dispose();
        }

        public Size Size => Source.Size;
        public Bitmap Source { get; }
        internal ICanvasImage Output => transformEffect;

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
            return new Win2DDrawingContext(renderTarget.CreateDrawingSession());
        }

        public void UpdateSource()
        {
            this.updating = true;
            Source.Pixels = renderTarget.GetPixelBytes();
            this.updating = false;
        }

        private void Source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Bitmap.Pixels):
                    UpdatePixels();
                    break;

                case nameof(Bitmap.Position):
                    UpdateTransform();
                    break;
            }
        }

        private void UpdatePixels()
        {
            if (!updating)
            {
                renderTarget.SetPixelBytes(Source.Pixels);
            }
        }

        private void UpdateTransform()
        {
            transformEffect.TransformMatrix = Matrix3x2.CreateTranslation(Source.Position.X, Source.Position.Y);
        }
    }
}
