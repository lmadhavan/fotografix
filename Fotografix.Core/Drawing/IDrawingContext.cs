using System;

namespace Fotografix.Drawing
{
    public interface IDrawingContext : IDisposable
    {
        void Draw(Image image);
        void Draw(BrushStroke brushStroke);
        void Draw(LinearGradient gradient);
    }
}
