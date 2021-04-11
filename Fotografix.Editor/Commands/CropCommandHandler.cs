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
                Crop(layer, rectangle);
            }

            image.Size = rectangle.Size;
        }

        private void Crop(Layer layer, Rectangle rectangle)
        {
            if (layer is BitmapLayer bitmapLayer)
            {
                bitmapLayer.Bitmap.Position -= (Size)rectangle.Location;
            }
        }
    }
}
