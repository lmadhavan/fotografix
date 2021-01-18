using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class ResampleImageCommandHandler : ICommandHandler<ResampleImageCommand>
    {
        private readonly IBitmapResamplingStrategy resamplingStrategy;

        public ResampleImageCommandHandler(IBitmapResamplingStrategy resamplingStrategy)
        {
            this.resamplingStrategy = resamplingStrategy;
        }

        public void Handle(ResampleImageCommand command)
        {
            Resample(command.Image, command.NewSize);
        }

        private void Resample(Image image, Size newSize)
        {
            image.Accept(new ImageResamplingVisitor(newSize, resamplingStrategy));
        }

        private sealed class ImageResamplingVisitor : ImageElementVisitor
        {
            private readonly Size newSize;
            private readonly IBitmapResamplingStrategy resamplingStrategy;
            private PointF scaleFactor;

            public ImageResamplingVisitor(Size newSize, IBitmapResamplingStrategy resamplingStrategy)
            {
                this.newSize = newSize;
                this.resamplingStrategy = resamplingStrategy;
            }

            public override bool VisitEnter(Image image)
            {
                Size oldSize = image.Size;
                this.scaleFactor = new PointF((float)newSize.Width / oldSize.Width,
                                              (float)newSize.Height / oldSize.Height);
                return true;
            }

            public override bool VisitEnter(BitmapLayer layer)
            {
                Bitmap oldBitmap = layer.Bitmap;
                Size resampledSize = new Size((int)(oldBitmap.Size.Width * scaleFactor.X), (int)(oldBitmap.Size.Height * scaleFactor.Y));
                layer.Bitmap = resamplingStrategy.Resample(oldBitmap, resampledSize);
                return false;
            }

            public override bool VisitLeave(Image image)
            {
                image.Size = newSize;
                return false;
            }
        }
    }
}
