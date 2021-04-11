using System.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed class CropCommandHandler : ICommandHandler<CropCommand>
    {
        public void Handle(CropCommand command)
        {
            Crop(command.Image, command.Rectangle);
        }

        private void Crop(Image image, Rectangle rectangle)
        {
            foreach (Layer layer in image.Layers)
            {
                Crop(layer.Content, rectangle);
            }

            image.Size = rectangle.Size;
        }

        private void Crop(ContentElement content, Rectangle rectangle)
        {
            if (content is Bitmap bitmap)
            {
                bitmap.Position -= (Size)rectangle.Location;
            }
        }
    }
}
