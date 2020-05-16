using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Win2D
{
    internal sealed class BrushStrokePainter
    {
        private readonly float size;
        private readonly Windows.UI.Color color;
        private readonly IReadOnlyList<PointF> points;

        public BrushStrokePainter(BrushStroke brushStroke)
        {
            this.size = brushStroke.Size;
            this.color = Convert(brushStroke.Color);
            this.points = brushStroke.Points;
        }

        public void Paint(CanvasDrawingSession ds)
        {
            DrawCircle(ds, points[0]);

            for (int i = 1; i < points.Count; i++)
            {
                DrawLine(ds, points[i - 1], points[i]);
                DrawCircle(ds, points[i]);
            }
        }

        private void DrawCircle(CanvasDrawingSession ds, PointF pt)
        {
            ds.FillCircle(pt.X, pt.Y, size / 2, color);
        }

        private void DrawLine(CanvasDrawingSession ds, PointF pt1, PointF pt2)
        {
            ds.DrawLine(pt1.X, pt1.Y, pt2.X, pt2.Y, color, size);
        }

        private static Windows.UI.Color Convert(Color color)
        {
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
