using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class ResampleImageCommand : Command
    {
        private readonly Image image;
        private readonly Size newSize;
        private readonly IBitmapResamplingStrategy resamplingStrategy;

        private Size oldSize;
        private List<Bitmap> oldBitmaps;

        public ResampleImageCommand(Image image, Size newSize, IBitmapResamplingStrategy resamplingStrategy)
        {
            this.image = image;
            this.newSize = newSize;
            this.resamplingStrategy = resamplingStrategy;
        }

        public override bool IsEffective => image.Size != newSize;

        public override void Execute()
        {
            this.oldSize = image.Size;
            this.oldBitmaps = new List<Bitmap>();

            PointF ratio = new PointF((float)newSize.Width / oldSize.Width,
                                      (float)newSize.Height / oldSize.Height);

            foreach (Layer layer in image.Layers)
            {
                if (layer is BitmapLayer bitmapLayer)
                {
                    Bitmap oldBitmap = bitmapLayer.Bitmap;
                    Bitmap newBitmap = resamplingStrategy.Resample(oldBitmap, Scale(oldBitmap.Size, ratio));

                    oldBitmaps.Add(oldBitmap);
                    bitmapLayer.Bitmap = newBitmap;
                }
            }

            image.Size = newSize;
        }

        public override void Undo()
        {
            IEnumerator<Bitmap> e = oldBitmaps.GetEnumerator();

            foreach (Layer layer in image.Layers)
            {
                if (layer is BitmapLayer bitmapLayer)
                {
                    e.MoveNext();
                    bitmapLayer.Bitmap = e.Current;
                }
            }

            image.Size = oldSize;
        }

        private static Size Scale(Size size, PointF ratio)
        {
            return new Size((int)(size.Width * ratio.X), (int)(size.Height * ratio.Y));
        }
    }
}