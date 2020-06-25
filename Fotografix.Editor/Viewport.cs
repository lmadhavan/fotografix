using System;
using System.Drawing;

namespace Fotografix.Editor
{
    public abstract class Viewport
    {
        public abstract int Width { get; }
        public abstract int Height { get; }

        public abstract float ZoomFactor { get; set; }
        public abstract PointF ScrollOffset { get; set; }

        public void ScrollContentBy(PointF delta)
        {
            PointF offset = ScrollOffset;
            offset.X += ZoomFactor * delta.X;
            offset.Y += ZoomFactor * delta.Y;
            this.ScrollOffset = offset;
        }

        public void ZoomToFit(Size size)
        {
            float zx = (float)Width / size.Width;
            float zy = (float)Height / size.Height;
            this.ZoomFactor = Math.Min(1, Math.Min(zx, zy));
        }
    }
}
