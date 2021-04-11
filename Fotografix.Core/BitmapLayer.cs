using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class BitmapLayer : Layer
    {
        private Bitmap bitmap;

        public BitmapLayer() : this(new Bitmap(Size.Empty))
        {
        }

        public BitmapLayer(Bitmap bitmap)
        {
            this.Bitmap = bitmap;
        }

        public Bitmap Bitmap
        {
            get
            {
                return bitmap;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                SetChild(ref bitmap, value);
            }
        }

        public override ImageElement Content => Bitmap;
    }
}
