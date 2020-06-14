using System;
using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class ResizeImageParameters : NotifyPropertyChangedBase
    {
        private readonly int originalWidth;
        private readonly int originalHeight;

        private int width;
        private int height;
        private bool lockAspectRatio;

        public ResizeImageParameters(Size size)
        {
            this.originalWidth = this.width = size.Width;
            this.originalHeight = this.height = size.Height;
            this.lockAspectRatio = true;
        }

        public Size Size => new Size(width, height);

        public int Width
        {
            get
            {
                return width;
            }

            set
            {
                if (SetProperty(ref width, value))
                {
                    AdjustHeight();
                }
            }
        }

        public int Height
        {
            get
            {
                return height;
            }

            set
            {
                if (SetProperty(ref height, value))
                {
                    AdjustWidth();
                }
            }
        }

        public bool LockAspectRatio
        {
            get
            {
                return lockAspectRatio;
            }

            set
            {
                if (SetProperty(ref lockAspectRatio, value))
                {
                    // adjust one of the dimensions so that it honors the new value of lockAspectRatio
                    AdjustHeight();
                }
            }
        }

        private void AdjustWidth()
        {
            if (lockAspectRatio)
            {
                this.Width = Math.Max(1, height * originalWidth / originalHeight);
            }
        }

        private void AdjustHeight()
        {
            if (lockAspectRatio)
            {
                this.Height = Math.Max(1, width * originalHeight / originalWidth);
            }
        }
    }
}
