using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class Bitmap
    {
        public static readonly Bitmap Empty = new Bitmap(Size.Empty);

        public Bitmap(Size size)
        {
            this.Size = size;
            this.Pixels = new byte[GetBufferLength(size)];
        }

        public Bitmap(Size size, byte[] pixels)
        {
            int bufferLength = GetBufferLength(size);

            if (pixels.Length != bufferLength)
            {
                throw new ArgumentException($"Invalid size for pixel array: expected = {bufferLength}, actual = {pixels.Length}", nameof(pixels));
            }

            this.Size = size;
            this.Pixels = pixels;
        }

        public Size Size { get; }
        public byte[] Pixels { get; }

        public void Paint(BrushStroke brushStroke)
        {
            throw new NotImplementedException();
        }

        private static int GetBufferLength(Size size)
        {
            return size.Width * size.Height * 4;
        }
    }
}
