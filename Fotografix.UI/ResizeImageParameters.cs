using System.Drawing;

namespace Fotografix.UI
{
    public sealed class ResizeImageParameters : NotifyPropertyChangedBase
    {
        private int width;
        private int height;

        public ResizeImageParameters(Size size)
        {
            this.width = size.Width;
            this.height = size.Height;
        }

        public Size Size => new Size(Width, Height);

        public int Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        public int Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }
    }
}
