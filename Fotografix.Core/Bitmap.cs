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

        public override string ToString()
        {
            return $"Bitmap {Size}";
        }
    }
}
