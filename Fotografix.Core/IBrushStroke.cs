using System.Drawing;

namespace Fotografix
{
    public interface IBrushStroke : IDrawable
    {
        void AddPoint(Point pt);
    }
}