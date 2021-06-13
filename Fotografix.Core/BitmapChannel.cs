using Fotografix.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Fotografix
{
    public sealed class BitmapChannel : Channel
    {
        private Bitmap bitmap;

        public BitmapChannel(Bitmap bitmap)
        {
            this.Bitmap = bitmap;
        }

        public override ImageElement Content => bitmap;

        public Bitmap Bitmap
        {
            get
            {
                return bitmap;
            }

            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                if (bitmap != null)
                {
                    bitmap.PropertyChanged -= Bitmap_PropertyChanged;
                }

                if (SetChild(ref bitmap, value))
                {
                    RaisePropertyChanged(nameof(Content));
                }

                if (bitmap != null)
                {
                    bitmap.PropertyChanged += Bitmap_PropertyChanged;
                }
            }
        }

        public override bool CanDraw => true;
        public override bool CanMove => true;

        public override Point Position
        {
            get => bitmap.Position;
            set => bitmap.Position = value;
        }

        public override void Draw(IDrawable drawable, IGraphicsDevice graphicsDevice)
        {
            Bitmap target = ResolveTargetBitmap(bitmap, drawable, out bool redrawExistingBitmap);

            using (IDrawingContext dc = graphicsDevice.CreateDrawingContext(target))
            {
                if (redrawExistingBitmap)
                {
                    dc.Draw(bitmap, destRect: bitmap.Bounds, srcRect: bitmap.Bounds);
                }

                drawable.Draw(dc);
            }

            this.Bitmap = target;
        }

        public override void Crop(Rectangle rectangle)
        {
            bitmap.Position -= (Size)rectangle.Location;
        }

        public override void Scale(PointF scaleFactor, IGraphicsDevice graphicsDevice)
        {
            Rectangle bounds = bitmap.Bounds;

            Rectangle scaledBounds = new((int)(bounds.X * scaleFactor.X),
                                         (int)(bounds.Y * scaleFactor.Y),
                                         (int)(bounds.Width * scaleFactor.X),
                                         (int)(bounds.Height * scaleFactor.Y));

            Bitmap scaledBitmap = new Bitmap(scaledBounds);
            using (IDrawingContext dc = graphicsDevice.CreateDrawingContext(scaledBitmap))
            {
                dc.Draw(bitmap, destRect: scaledBitmap.Bounds, srcRect: bitmap.Bounds);
            }

            this.Bitmap = scaledBitmap;
        }

        private Bitmap ResolveTargetBitmap(Bitmap bitmap, IDrawable drawable, out bool redrawExistingBitmap)
        {
            redrawExistingBitmap = false;

            if (bitmap.Bounds.IsEmpty)
            {
                return new Bitmap(drawable.Bounds);
            }

            if (!bitmap.Bounds.Contains(drawable.Bounds))
            {
                redrawExistingBitmap = true;
                return new Bitmap(Rectangle.Union(bitmap.Bounds, drawable.Bounds));
            }

            return bitmap;
        }

        private void Bitmap_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Bitmap.Position))
            {
                RaisePropertyChanged(nameof(Position));
            }
        }
    }
}
