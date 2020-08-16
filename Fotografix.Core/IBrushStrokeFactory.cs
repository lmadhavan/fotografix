using System.Drawing;

namespace Fotografix
{
    public interface IBrushStrokeFactory
    {
        IBrushStroke CreateBrushStroke(Point start, int size, Color color);
    }
}
