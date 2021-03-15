using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor.Drawing
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

        private void Draw(BitmapLayer layer, IDrawable drawable)
        {
            Bitmap target = ResolveTargetBitmap(layer.Bitmap, drawable, out bool redrawExistingBitmap);

            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(target))
            {
                if (redrawExistingBitmap)
                {
                    dc.Draw(layer.Bitmap);
                }

                drawable.Draw(dc);
            }

            layer.Bitmap = target;
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
