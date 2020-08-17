using System.Drawing;

namespace Fotografix.Drawing
{
    public interface IBrushStrokeFactory
    {
        IBrushStroke CreateBrushStroke(Point start, int size, Color color);
    }
}
