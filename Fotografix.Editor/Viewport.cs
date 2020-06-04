using System;
using System.Drawing;

namespace Fotografix.Editor
{
    public abstract class Viewport
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        
        public abstract float ZoomFactor { get; set; }

        public void ZoomToFit(Size size)
        {
            float zx = (float)Width / size.Width;
            float zy = (float)Height / size.Height;
            this.ZoomFactor = Math.Min(1, Math.Min(zx, zy));
        }
    }
}
