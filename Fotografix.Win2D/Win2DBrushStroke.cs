using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Win2D
{
    public sealed class Win2DBrushStroke : NotifyContentChangedBase, IBrushStroke, IWin2DDrawable
    {
        private readonly List<Point> points;
        private readonly float size;
        private readonly Windows.UI.Color color;

        public Win2DBrushStroke(Point start, int size, Color color)
        {
            this.points = new List<Point> { start };
            this.size = size;
            this.color = Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public void Dispose()
        {
        }

        public void AddPoint(Point pt)
        {
            points.Add(pt);
            RaiseContentChanged();
        }

        public void Draw(CanvasDrawingSession ds)
        {
            FillCircle(ds, points[0]);

            for (int i = 1; i < points.Count; i++)
            {
                DrawLine(ds, points[i - 1], points[i]);
                FillCircle(ds, points[i]);
            }
        }

        private void FillCircle(CanvasDrawingSession ds, PointF pt)
        {
            ds.FillCircle(pt.X, pt.Y, size / 2, color);
        }

        private void DrawLine(CanvasDrawingSession ds, PointF pt1, PointF pt2)
        {
            ds.DrawLine(pt1.X, pt1.Y, pt2.X, pt2.Y, color, size);
        }
    }
}
