using Fotografix.Drawing;
using System;
using System.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed class DrawCommandHandler : ICommandHandler<DrawCommand>
    {
        private readonly IDrawingContextFactory drawingContextFactory;

        public DrawCommandHandler(IDrawingContextFactory drawingContextFactory)
        {
            this.drawingContextFactory = drawingContextFactory;
        }

        public void Handle(DrawCommand command)
        {
            Draw(command.Layer, command.Drawable);
        }

        private void Draw(Layer layer, IDrawable drawable)
        {
            Bitmap bitmap = layer.Content as Bitmap ?? throw new InvalidOperationException("Cannot draw on layer with content type " + layer.Content.GetType());
            Bitmap target = ResolveTargetBitmap(bitmap, drawable, out bool redrawExistingBitmap);

            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(target))
            {
                if (redrawExistingBitmap)
                {
                    dc.Draw(bitmap);
                }

                drawable.Draw(dc);
            }

            layer.Content = target;
        }

        private Bitmap ResolveTargetBitmap(Bitmap bitmap, IDrawable drawable, out bool redrawExistingBitmap)
        {
            redrawExistingBitmap = false;

            if (bitmap.Bounds.IsEmpty)
            {
                return new Bitmap(drawable.Bounds);
            }

            if (!bitmap.Bounds.Contains(drawable.Bounds))
            {
                redrawExistingBitmap = true;
                return new Bitmap(Rectangle.Union(bitmap.Bounds, drawable.Bounds));
            }

            return bitmap;
        }
    }
}
