using System.Drawing;

namespace Fotografix.Win2D
{
    public sealed class Win2DBrushStrokeFactory : IBrushStrokeFactory
    {
        public IBrushStroke CreateBrushStroke(Point start, int size, Color color)
        {
            return new Win2DBrushStroke(start, size, color);
        }
    }
}
