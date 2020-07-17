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

        public override Bitmap Scale(Size newSize)
        {
            return new FakeBitmap(newSize);
        }

        public override void Paint(BrushStroke brushStroke)
        {
        }

        public override void Draw(Image image)
        {
        }
    }
}
