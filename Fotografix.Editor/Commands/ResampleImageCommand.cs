using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed class ResampleImageCommand : ICommand
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

        public IChange PrepareChange()
        {
            Size oldSize = image.Size;
            PointF ratio = new PointF((float)newSize.Width / oldSize.Width,
                                      (float)newSize.Height / oldSize.Height);

            List<Bitmap> oldBitmaps = new List<Bitmap>();
            List<Bitmap> newBitmaps = new List<Bitmap>();

            foreach (BitmapLayer layer in image.Layers)
            {
                Bitmap oldBitmap = layer.Bitmap;
                Bitmap newBitmap = resamplingStrategy.Resample(oldBitmap, Scale(oldBitmap.Size, ratio));

                oldBitmaps.Add(oldBitmap);
                newBitmaps.Add(newBitmap);
            }

            return new ResampleImageChange(image, oldSize, newSize, oldBitmaps, newBitmaps);
        }

        private Size Scale(Size size, PointF ratio)
        {
            return new Size((int)(size.Width * ratio.X), (int)(size.Height * ratio.Y));
        }

        private sealed class ResampleImageChange : IChange
        {
            private readonly Image image;

            private readonly Size oldSize;
            private readonly Size newSize;

            private readonly IReadOnlyList<Bitmap> oldBitmaps;
            private readonly IReadOnlyList<Bitmap> newBitmaps;

            public ResampleImageChange(Image image, Size oldSize, Size newSize, List<Bitmap> oldBitmaps, List<Bitmap> newBitmaps)
            {
                this.image = image;
                this.oldSize = oldSize;
                this.newSize = newSize;
                this.oldBitmaps = oldBitmaps;
                this.newBitmaps = newBitmaps;
            }

            public void Apply()
            {
                image.Size = newSize;

                int i = 0;
                foreach (BitmapLayer layer in image.Layers)
                {
                    layer.Bitmap = newBitmaps[i++];
                }
            }

            public void Undo()
            {
                image.Size = oldSize;

                int i = 0;
                foreach (BitmapLayer layer in image.Layers)
                {
                    layer.Bitmap = oldBitmaps[i++];
                }
            }
        }
    }
}