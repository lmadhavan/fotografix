using System.Drawing;

namespace Fotografix.Testing
{
    public class FakeBitmap : Bitmap
    {
        private byte[] pixels;

        public FakeBitmap() : this(Size.Empty)
        {
        }

        public FakeBitmap(Size size) : base(size)
        {
        }

        public override byte[] GetPixelBytes()
        {
            return pixels;
        }

        public override void SetPixelBytes(byte[] pixels)
        {
            this.pixels = pixels;
        }
    }
}
