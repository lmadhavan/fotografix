using System;
using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class Viewport : NotifyPropertyChangedBase
    {
        private Size size;
        private Size imageSize;
        private float zoomFactor;
        private bool zoomToFit;
        private Point scrollOffset;

        private bool computingOffsets;
        private Point maxScrollOffset;
        private Point centerOffset;

        public Viewport() : this(Size.Empty)
        {
        }

        public Viewport(Size size)
        {
            this.size = size;
            this.imageSize = size;
            this.zoomFactor = 1;
        }

        public Size Size
        {
            get => size;

            set
            {
                if (SetProperty(ref size, value))
                {
                    ComputeOffsets();
                }
            }
        }

        public Size ImageSize
        {
            get => imageSize;

            set
            {
                if (SetProperty(ref imageSize, value))
                {
                    ComputeOffsets();
                }
            }
        }

        public float ZoomFactor
        {
            get => zoomFactor;

            set
            {
                if (SetProperty(ref zoomFactor, value))
                {
                    if (!computingOffsets)
                    {
                        // manually setting zoom factor turns off zoom to fit
                        this.ZoomToFit = false;
                        ComputeOffsets();
                    }
                }
            }
        }

        public bool ZoomToFit
        {
            get => zoomToFit;

            set
            {
                if (SetProperty(ref zoomToFit, value))
                {
                    ComputeOffsets();
                }
            }
        }

        public Point ScrollOffset
        {
            get => scrollOffset;
            set => SetProperty(ref scrollOffset, Clamp(value, Point.Empty, maxScrollOffset));
        }

        public Rectangle ImageBounds => TransformImageToViewport(new Rectangle(Point.Empty, ImageSize));

        public Point TransformViewportToImage(Point viewportPoint)
        {
            return new Point(
                (int)((viewportPoint.X + ScrollOffset.X - centerOffset.X) / zoomFactor),
                (int)((viewportPoint.Y + ScrollOffset.Y - centerOffset.Y) / zoomFactor)
            );
        }

        public Rectangle TransformViewportToImage(Rectangle viewportRect)
        {
            return new Rectangle(
                TransformViewportToImage(viewportRect.Location),
                Scale(viewportRect.Size, 1 / zoomFactor)
            );
        }

        public Point TransformImageToViewport(Point imagePoint)
        {
            return new Point(
                (int)(imagePoint.X * zoomFactor) - ScrollOffset.X + centerOffset.X,
                (int)(imagePoint.Y * zoomFactor) - ScrollOffset.Y + centerOffset.Y
            );
        }

        public Rectangle TransformImageToViewport(Rectangle imageRect)
        {
            return new Rectangle(
                TransformImageToViewport(imageRect.Location),
                Scale(imageRect.Size, zoomFactor)
            );
        }

        private void ComputeOffsets()
        {
            this.computingOffsets = true;

            if (zoomToFit)
            {
                float zx = (float)size.Width / imageSize.Width;
                float zy = (float)size.Height / imageSize.Height;
                this.ZoomFactor = Math.Min(1, Math.Min(zx, zy));
            }

            int dx = (int)(imageSize.Width * zoomFactor - size.Width);
            int dy = (int)(imageSize.Height * zoomFactor - size.Height);

            this.maxScrollOffset = new Point(
                Math.Max(0, dx),
                Math.Max(0, dy)
            );

            this.centerOffset = new Point(
                Math.Max(0, -dx / 2),
                Math.Max(0, -dy / 2)
            );

            // ensure scroll offset is still valid
            this.ScrollOffset = ScrollOffset;

            this.computingOffsets = false;
        }

        private Point Clamp(Point pt, Point min, Point max)
        {
            return new Point(
                Clamp(pt.X, min.X, max.X),
                Clamp(pt.Y, min.Y, max.Y)
            );
        }

        private int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        private Size Scale(Size size, float factor)
        {
            return new Size(
                (int)(size.Width * factor),
                (int)(size.Height * factor)
            );
        }
    }
}
