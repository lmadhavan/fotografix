using System.Drawing;

namespace Fotografix.Drawing
{
    public interface IBrushStroke : IDrawable
    {
        void AddPoint(Point pt);
    }
}