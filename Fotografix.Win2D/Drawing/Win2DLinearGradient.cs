using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System.Drawing;

namespace Fotografix.Win2D.Drawing
{
    public sealed class Win2DLinearGradient : NotifyContentChangedBase, IGradient, IWin2DDrawable
    {
        private readonly CanvasLinearGradientBrush brush;

        public Win2DLinearGradient(ICanvasResourceCreator resourceCreator, Color startColor, Color endColor, Point startPoint)
        {
            this.brush = new CanvasLinearGradientBrush(resourceCreator, startColor.ToWindowsColor(), endColor.ToWindowsColor());
            brush.StartPoint = startPoint.ToVector2();
        }

        public void Dispose()
        {
            brush.Dispose();
        }

        public void SetEndPoint(Point pt)
        {
            brush.EndPoint = pt.ToVector2();
            RaiseContentChanged();
        }

        public void Draw(CanvasDrawingSession ds, Rectangle bounds)
        {
            ds.FillRectangle(bounds.ToWindowsRect(), brush);
        }
    }
}