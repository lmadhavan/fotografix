﻿using System;

namespace Fotografix
{
    public sealed class BitmapLayer : Layer
    {
        private Bitmap bitmap;

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

                SetProperty(ref bitmap, value);
            }
        }

        public override bool CanPaint => true;

        public override void Accept(LayerVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Paint(BrushStroke brushStroke)
        {
            bitmap.Paint(brushStroke);
        }
    }
}
