using System;
using System.Drawing;

namespace Fotografix.Drawing
{
    public sealed class TestDrawable : IDrawable
    {
        public TestDrawable(Rectangle bounds)
        {
            this.Bounds = bounds;
        }

        public Rectangle Bounds { get; }

        public event EventHandler Changed;

        public void Draw(IDrawingContext drawingContext)
        {
            ((TestGraphicsDevice)drawingContext).Draw(this);
        }

        public void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
