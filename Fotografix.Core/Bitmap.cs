using System;
using System.Drawing;

namespace Fotografix
{
    public abstract class Bitmap : IDisposable
    {
        protected Bitmap(Size size)
        {
            this.Size = size;
        }

        public virtual void Dispose()
        {
        }

        public Size Size { get; }

        public abstract byte[] GetPixelBytes();
        public abstract void SetPixelBytes(byte[] pixels);

        public Bitmap Scale(PointF ratio)
        {
            Size newSize = new Size((int)(Size.Width * ratio.X), (int)(Size.Height * ratio.Y));
            return Scale(newSize);
        }

        public abstract Bitmap Scale(Size newSize);
        public abstract void Paint(BrushStroke brushStroke);
        public abstract void Draw(Image image);
    }
}
