using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class Bitmap : ContentElement
    {
        private Point position = Point.Empty;
        private byte[] pixels;

        public Bitmap(Size size)
        {
            this.Size = size;
            this.pixels = new byte[BufferLength(size)];
        }

        public Bitmap(Rectangle rectangle) : this(rectangle.Size)
        {
            this.position = rectangle.Location;
        }

        public Bitmap(Size size, byte[] pixels)
        {
            this.Size = size;
            ValidateBufferLength(pixels);
            this.pixels = pixels;
        }

        public Point Position
        {
            get
            {
                return position;
            }

            set
            {
                if (SetProperty(ref position, value))
                {
                    RaisePropertyChanged(nameof(Bounds));
                }
            }
        }

        public Size Size { get; }

        public Rectangle Bounds => new Rectangle(Position, Size);

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
            return $"Bitmap {Bounds}";
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
