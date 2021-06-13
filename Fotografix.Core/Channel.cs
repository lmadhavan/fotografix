using Fotografix.Drawing;
using System;
using System.Drawing;

namespace Fotografix
{
    public abstract class Channel : ImageElement
    {
        public abstract ImageElement Content { get; }

        public virtual bool CanDraw => false;
        public virtual bool CanMove => false;

        public virtual Point Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public virtual void Draw(IDrawable drawable, IDrawingContextFactory drawingContextFactory)
        {
            throw new NotSupportedException();
        }

        public abstract void Crop(Rectangle rectangle);
        public abstract void Scale(PointF scaleFactor, IDrawingContextFactory drawingContextFactory);
    }
}
