using System;

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

        public override IUndoable Paint(BrushStroke brushStroke)
        {
            BitmapState bitmapState = new BitmapState(bitmap);
            bitmap.Paint(brushStroke);
            return bitmapState;
        }

        private sealed class BitmapState : IUndoable
        {
            private readonly Bitmap bitmap;
            private readonly byte[] pixels;

            public BitmapState(Bitmap bitmap)
            {
                this.bitmap = bitmap;
                this.pixels = bitmap.GetPixelBytes();
            }

            public void Undo()
            {
                bitmap.SetPixelBytes(pixels);
            }
        }
    }
}
