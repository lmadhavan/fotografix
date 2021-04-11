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
                layer.Content = Resample(layer.Content, scaleFactor);
            }

            image.Size = newSize;
        }

        private ContentElement Resample(ContentElement content, PointF scaleFactor)
        {
            if (content is Bitmap bitmap)
            {
                Size resampledSize = new((int)(bitmap.Size.Width * scaleFactor.X),
                                         (int)(bitmap.Size.Height * scaleFactor.Y));
                return resamplingStrategy.Resample(bitmap, resampledSize);
            }

            return content;
        }
    }
}
