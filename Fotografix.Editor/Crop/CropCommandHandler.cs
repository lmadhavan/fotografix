using System.Drawing;

namespace Fotografix.Editor.Crop
{
    public sealed class CropCommandHandler : ICommandHandler<CropCommand>
    {
        public void Handle(CropCommand command)
        {
            Crop(command.Image, command.Rectangle);
        }

        private void Crop(Image image, Rectangle rectangle)
        {
            image.Size = rectangle.Size;

            foreach (Layer layer in image.Layers)
            {
                layer.Position -= (Size)rectangle.Location;
            }
        }
    }
}
