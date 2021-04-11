using System.Drawing;

namespace Fotografix.Editor.Commands
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
            Size oldSize = image.Size;
            PointF scaleFactor = new((float)newSize.Width / oldSize.Width,
                                     (float)newSize.Height / oldSize.Height);

            foreach (Layer layer in image.Layers)
            {
                Resample(layer, scaleFactor);
            }

            image.Size = newSize;
        }

        private void Resample(Layer layer, PointF scaleFactor)
        {
            if (layer is BitmapLayer bitmapLayer)
            {
                Bitmap oldBitmap = bitmapLayer.Bitmap;
                Size resampledSize = new((int)(oldBitmap.Size.Width * scaleFactor.X),
                                         (int)(oldBitmap.Size.Height * scaleFactor.Y));
                bitmapLayer.Bitmap = resamplingStrategy.Resample(oldBitmap, resampledSize);
            }
        }
    }
}
