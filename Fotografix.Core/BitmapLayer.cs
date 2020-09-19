using Fotografix.Drawing;
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

        public override IUndoable Draw(IDrawingContextFactory drawingContextFactory, IDrawable drawable)
        {
            BitmapState bitmapState = new BitmapState(this);
            using (IDrawingContext dc = drawingContextFactory.CreateDrawingContext(bitmap))
            {
                drawable.Draw(dc);
            }
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
                this.pixels = layer.bitmap.ClonePixels();
            }

            public void Undo()
            {
                layer.bitmap.SetPixels(pixels);
                layer.RaiseContentChanged();
            }
        }
    }
}
