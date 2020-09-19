using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class Bitmap : INotifyContentChanged
    {
        public static readonly Bitmap Empty = new Bitmap(Size.Empty);

        public Bitmap(Size size)
        {
            this.Size = size;
            this.Pixels = new byte[BufferLength(size)];
        }

        public Bitmap(Size size, byte[] pixels)
        {
            this.Size = size;
            ValidateBufferLength(pixels);
            this.Pixels = pixels;
        }

        public Size Size { get; }
        public byte[] Pixels { get; }

        public event EventHandler<ContentChangedEventArgs> ContentChanged;

        public byte[] ClonePixels()
        {
            return (byte[])Pixels.Clone();
        }

        public void SetPixels(byte[] pixels)
        {
            ValidateBufferLength(pixels);
            Array.Copy(pixels, Pixels, Pixels.Length);
            Invalidate();
        }

        public void Invalidate()
        {
            ContentChanged?.Invoke(this, new ContentChangedEventArgs());
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
