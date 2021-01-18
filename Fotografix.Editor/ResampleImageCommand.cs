using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class ResampleImageCommand : ICommand
    {
        public ResampleImageCommand(Image image, Size newSize)
        {
            this.Image = image;
            this.NewSize = newSize;
        }

        public Image Image { get; }
        public Size NewSize { get; }
    }
}