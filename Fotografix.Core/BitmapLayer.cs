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

        public override IUndoable Paint(BrushStroke brushStroke)
        {
            BitmapState bitmapState = new BitmapState(this);
            bitmap.Paint(brushStroke);
            RaiseContentChanged();
            return bitmapState;
        }

        private sealed class BitmapState : IUndoable
        {
            private readonly BitmapLayer layer;
            private readonly byte[] pixels;

            public BitmapState(BitmapLayer layer)
            {
                this.layer = layer;
                this.pixels = layer.bitmap.GetPixelBytes();
            }

            public void Undo()
            {
                layer.bitmap.SetPixelBytes(pixels);
                layer.RaiseContentChanged();
            }
        }
    }
}
