using System;
using System.Drawing;

namespace Fotografix.Editor.Tests
{
    public sealed class FakeBitmap : Bitmap
    {
        public FakeBitmap() : this(Size.Empty)
        {
        }

        public FakeBitmap(Size size) : base(size)
        {
        }

        public override byte[] GetPixelBytes()
        {
            throw new NotImplementedException();
        }

        public override void SetPixelBytes(byte[] pixels)
        {
            throw new NotImplementedException();
        }

        public override Bitmap Scale(Size newSize)
        {
            return new FakeBitmap(newSize);
        }

        public override void Paint(BrushStroke brushStroke)
        {
            throw new NotImplementedException();
        }
    }
}
