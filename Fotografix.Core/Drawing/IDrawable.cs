using System;

namespace Fotografix.Drawing
{
    public interface IDrawable
    {
        void Draw(IDrawingContext drawingContext);

        event EventHandler Changed;
    }
}