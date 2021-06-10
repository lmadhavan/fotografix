using System;
using System.Drawing;

namespace Fotografix.Drawing
{
    public sealed class ClippedDrawable : IDrawable
    {
        private ClippedDrawable(IDrawable wrappedDrawable, Rectangle clipRectangle)
        {
            this.WrappedDrawable = wrappedDrawable;
            this.ClipRectangle = clipRectangle;
        }

        public IDrawable WrappedDrawable { get; }
        public Rectangle ClipRectangle { get; }

        public Rectangle Bounds => Rectangle.Intersect(WrappedDrawable.Bounds, ClipRectangle);

        public event EventHandler Changed
        {
            add => WrappedDrawable.Changed += value;
            remove => WrappedDrawable.Changed -= value;
        }

        public void Draw(IDrawingContext drawingContext)
        {
            using (drawingContext.BeginClip(ClipRectangle))
            {
                WrappedDrawable.Draw(drawingContext);
            }
        }

        public static IDrawable Create(IDrawable drawable, Rectangle clipRectangle)
        {
            if (clipRectangle.IsEmpty)
            {
                return drawable;
            }

            return new ClippedDrawable(drawable, clipRectangle);
        }
    }
}
