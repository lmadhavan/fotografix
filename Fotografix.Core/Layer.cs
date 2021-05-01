using Fotografix.Drawing;
using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class Layer : ImageElement
    {
        private string name = "";
        private bool visible = true;
        private BlendMode blendMode = BlendMode.Normal;
        private float opacity = 1;
        private ContentElement content;

        public Layer() : this(new Bitmap(Size.Empty))
        {
        }

        public Layer(ContentElement content)
        {
            this.Content = content;
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                SetProperty(ref name, value);
            }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }

            set
            {
                SetProperty(ref visible, value);
            }
        }

        public BlendMode BlendMode
        {
            get
            {
                return blendMode;
            }

            set
            {
                SetProperty(ref blendMode, value);
            }
        }

        public float Opacity
        {
            get
            {
                return opacity;
            }

            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref opacity, value);
            }
        }

        public ContentElement Content
        {
            get
            {
                return content;
            }

            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                SetChild(ref content, value);
            }
        }

        internal void Crop(Rectangle rectangle)
        {
            if (content is Bitmap bitmap)
            {
                bitmap.Position -= (Size)rectangle.Location;
            }
        }

        public void Draw(IDrawable drawable, IDrawingContextFactory drawingContextFactory)
        {
            Bitmap bitmap = content as Bitmap ?? throw new InvalidOperationException("Cannot draw on layer with content type " + content.GetType());
            Bitmap target = ResolveTargetBitmap(bitmap, drawable, out bool redrawExistingBitmap);

            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(target))
            {
                if (redrawExistingBitmap)
                {
                    dc.Draw(bitmap);
                }

                drawable.Draw(dc);
            }

            this.Content = target;
        }

        internal void Scale(PointF scaleFactor, IBitmapResamplingStrategy resamplingStrategy)
        {
            if (content is Bitmap bitmap)
            {
                Size resampledSize = new((int)(bitmap.Size.Width * scaleFactor.X),
                                         (int)(bitmap.Size.Height * scaleFactor.Y));
                this.Content = resamplingStrategy.Resample(bitmap, resampledSize);
            }
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
    }
}