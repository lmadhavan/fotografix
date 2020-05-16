using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class ResampleImageCommand : Command
    {
        private readonly Image image;
        private readonly Size newSize;

        private Size oldSize;
        private List<Bitmap> oldBitmaps;

        public ResampleImageCommand(Image image, Size newSize)
        {
            this.image = image;
            this.newSize = newSize;
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
                    Bitmap newBitmap = oldBitmap.Scale(ratio);

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

    }
}