using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using System.Drawing;

namespace Fotografix.Win2D.Drawing
{
    public sealed class Win2DDrawableFactory : IBrushStrokeFactory, IGradientFactory
    {
        public IBrushStroke CreateBrushStroke(Point start, int size, Color color)
        {
            return new Win2DBrushStroke(start, size, color);
        }

        public IGradient CreateLinearGradient(Color startColor, Color endColor, Point startPoint)
        {
            return new Win2DLinearGradient(CanvasDevice.GetSharedDevice(), startColor, endColor, startPoint);
        }
    }
}
