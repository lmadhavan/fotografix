using Fotografix.Drawing;
using System;
using System.Drawing;

namespace Fotografix
{
    public abstract class Channel : ImageElement
    {
        internal Channel() { }

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

        internal abstract void Crop(Rectangle rectangle);
        internal abstract void Scale(PointF scaleFactor, IBitmapResamplingStrategy resamplingStrategy);
    }
}
