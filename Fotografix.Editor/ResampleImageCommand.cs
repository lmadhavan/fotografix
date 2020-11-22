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
            Size oldSize = image.Size;

            PointF ratio = new PointF((float)newSize.Width / oldSize.Width,
                                      (float)newSize.Height / oldSize.Height);

            foreach (Layer layer in image.Layers)
            {
                if (layer is BitmapLayer bitmapLayer)
                {
                    Bitmap oldBitmap = bitmapLayer.Bitmap;

                    Size newSize = new Size((int)(oldBitmap.Size.Width * ratio.X), (int)(oldBitmap.Size.Height * ratio.Y));
                    Bitmap newBitmap = resamplingStrategy.Resample(oldBitmap, newSize);
                    bitmapLayer.Bitmap = newBitmap;
                }
            }

            image.Size = newSize;
        }
    }
}