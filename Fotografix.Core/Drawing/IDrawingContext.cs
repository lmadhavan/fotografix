using System;
using System.Drawing;

namespace Fotografix.Drawing
{
    public interface IDrawingContext : IDisposable
    {
        void Draw(Bitmap bitmap, Rectangle destRect, Rectangle srcRect);
        void Draw(BrushStroke brushStroke);
        void Draw(LinearGradient gradient);

        IDisposable BeginClip(Rectangle rect);
    }
}
