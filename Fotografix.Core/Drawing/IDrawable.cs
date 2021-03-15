using System;
using System.Drawing;

namespace Fotografix.Drawing
{
    public interface IDrawable
    {
        Rectangle Bounds { get; }
        event EventHandler Changed;

        void Draw(IDrawingContext drawingContext);
    }
}