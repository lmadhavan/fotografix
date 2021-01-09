using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class ResampleImageCommand : Command
    {
        private readonly Image image;
        private readonly Size newSize;
        private readonly IBitmapResamplingStrategy resamplingStrategy;

        public ResampleImageCommand(Image image, Size newSize, IBitmapResamplingStrategy resamplingStrategy)
        {
            this.image = image;
            this.newSize = newSize;
            this.resamplingStrategy = resamplingStrategy;
        }

        public override void Execute()
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