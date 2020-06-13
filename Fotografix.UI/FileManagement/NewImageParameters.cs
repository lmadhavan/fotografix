using System.Drawing;

namespace Fotografix.UI.FileManagement
{
    public sealed class NewImageParameters : NotifyPropertyChangedBase
    {
        private int width = 100;
        private int height = 100;

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

        public Size Size => new Size(width, height);
    }
}
