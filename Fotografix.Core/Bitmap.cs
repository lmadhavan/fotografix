using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class Bitmap : ImageElement
    {
        private byte[] pixels;

        public Bitmap(Size size)
        {
            this.Size = size;
            this.pixels = new byte[BufferLength(size)];
        }

        public Bitmap(Size size, byte[] pixels)
        {
            this.Size = size;
            ValidateBufferLength(pixels);
            this.pixels = pixels;
        }

        public Size Size { get; }

        public byte[] Pixels
        {
            get => pixels;

            set
            {
                ValidateBufferLength(value);
                SetProperty(ref pixels, value);
            }
        }

        public override string ToString()
        {
            return $"Bitmap {Size}";
        }

        private void ValidateBufferLength(byte[] buffer)
        {
            int expectedBufferLength = BufferLength(Size);

            if (buffer.Length != expectedBufferLength)
            {
                throw new ArgumentException($"Incorrect buffer length for bitmap of size {Size}: expected = {expectedBufferLength}, actual = {buffer.Length}");
            }
        }

        private static int BufferLength(Size size)
        {
            return size.Width * size.Height * 4;
        }
    }
}
