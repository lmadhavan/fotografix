using System;

namespace Fotografix.Drawing
{
    public interface IDrawingContext : IDisposable
    {
        void Draw(Bitmap bitmap);
        void Draw(BrushStroke brushStroke);
        void Draw(LinearGradient gradient);
    }
}
