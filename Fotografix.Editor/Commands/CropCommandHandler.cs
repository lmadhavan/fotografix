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
            image.Accept(new ImageCroppingVisitor(rectangle));
        }

        private sealed class ImageCroppingVisitor : ImageElementVisitor
        {
            private readonly Rectangle rectangle;

            public ImageCroppingVisitor(Rectangle rectangle)
            {
                this.rectangle = rectangle;
            }

            public override bool Visit(Bitmap bitmap)
            {
                bitmap.Position -= (Size)rectangle.Location;
                return true;
            }

            public override bool VisitLeave(Image image)
            {
                image.Size = rectangle.Size;
                return true;
            }
        }
    }
}
