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

            if (oldSize == newSize)
            {
                return null;
            }

            PointF ratio = new PointF((float)newSize.Width / oldSize.Width,
                                      (float)newSize.Height / oldSize.Height);

            BitmapResamplingVisitor visitor = new BitmapResamplingVisitor(resamplingStrategy, ratio);
            foreach (Layer layer in image.Layers)
            {
                layer.Accept(visitor);
            }

            return new ResampleImageChange(image, oldSize, newSize, visitor.OldBitmaps, visitor.NewBitmaps);
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

                BitmapUpdatingVisitor visitor = new BitmapUpdatingVisitor(newBitmaps);
                foreach (Layer layer in image.Layers)
                {
                    layer.Accept(visitor);
                }
            }

            public void Undo()
            {
                image.Size = oldSize;

                BitmapUpdatingVisitor visitor = new BitmapUpdatingVisitor(oldBitmaps);
                foreach (Layer layer in image.Layers)
                {
                    layer.Accept(visitor);
                }
            }
        }

        private sealed class BitmapResamplingVisitor : ILayerVisitor
        {
            private readonly IBitmapResamplingStrategy resamplingStrategy;
            private readonly PointF ratio;

            public BitmapResamplingVisitor(IBitmapResamplingStrategy resamplingStrategy, PointF ratio)
            {
                this.resamplingStrategy = resamplingStrategy;
                this.ratio = ratio;
            }

            public List<Bitmap> OldBitmaps { get; } = new List<Bitmap>();
            public List<Bitmap> NewBitmaps { get; } = new List<Bitmap>();

            public void Visit(AdjustmentLayer layer)
            {
            }

            public void Visit(BitmapLayer layer)
            {
                Bitmap oldBitmap = layer.Bitmap;
                Bitmap newBitmap = resamplingStrategy.Resample(oldBitmap, Scale(oldBitmap.Size, ratio));

                OldBitmaps.Add(oldBitmap);
                NewBitmaps.Add(newBitmap);
            }

            private static Size Scale(Size size, PointF ratio)
            {
                return new Size((int)(size.Width * ratio.X), (int)(size.Height * ratio.Y));
            }
        }

        private sealed class BitmapUpdatingVisitor : ILayerVisitor
        {
            private readonly IEnumerator<Bitmap> bitmaps;

            public BitmapUpdatingVisitor(IReadOnlyList<Bitmap> bitmaps)
            {
                this.bitmaps = bitmaps.GetEnumerator();
            }

            public void Visit(AdjustmentLayer layer)
            {
            }

            public void Visit(BitmapLayer layer)
            {
                bitmaps.MoveNext();
                layer.Bitmap = bitmaps.Current;
            }
        }
    }
}