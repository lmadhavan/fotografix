using System;

namespace Fotografix
{
    public sealed class BitmapLayer : Layer
    {
        private IBitmap bitmap;

        public BitmapLayer(IBitmap bitmap)
        {
            this.Bitmap = bitmap;
        }

        public IBitmap Bitmap
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

                SetProperty(ref bitmap, value);
            }
        }
    }
}
