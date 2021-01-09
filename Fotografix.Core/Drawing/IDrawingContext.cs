using System;

namespace Fotografix.Drawing
{
    public interface IDrawingContext : IDisposable
    {
        void Draw(BrushStroke brushStroke);
        void Draw(LinearGradient gradient);
    }
}
