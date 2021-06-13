using Fotografix.Drawing;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Drawing;

namespace Fotografix.Win2D
{
    internal sealed class Win2DDrawingContext : IDrawingContext
    {
        private readonly CanvasDrawingSession ds;

        internal Win2DDrawingContext(CanvasDrawingSession ds)
        {
            this.ds = ds;
        }

        public void Dispose()
        {
            ds.Dispose();
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Disposed;

        public void Draw(Bitmap bitmap, Rectangle destRect, Rectangle srcRect)
        {
            using (Win2DBitmap win2DBitmap = new Win2DBitmap(bitmap, ds))
            {
                ds.DrawImage(win2DBitmap.Output, destRect.ToWindowsRect(), srcRect.ToWindowsRect());
            }
        }

        public void Draw(BrushStroke brushStroke)
        {
            var points = brushStroke.Points;
            var color = brushStroke.Color.ToWindowsColor();
            float width = brushStroke.Size;
            float radius = width / 2;

            void FillCircle(PointF pt) => ds.FillCircle(pt.X, pt.Y, radius, color);
            void DrawLine(PointF pt1, PointF pt2) => ds.DrawLine(pt1.X, pt1.Y, pt2.X, pt2.Y, color, width);

            FillCircle(points[0]);

            for (int i = 1; i < points.Count; i++)
            {
                DrawLine(points[i - 1], points[i]);
                FillCircle(points[i]);
            }
        }

        public void Draw(LinearGradient gradient)
        {
            using (var brush = new CanvasLinearGradientBrush(ds, gradient.StartColor.ToWindowsColor(), gradient.EndColor.ToWindowsColor()))
            {
                brush.StartPoint = gradient.StartPoint.ToVector2();
                brush.EndPoint = gradient.EndPoint.ToVector2();
                ds.FillRectangle(gradient.Bounds.ToWindowsRect(), brush);
            }
        }

        public IDisposable BeginClip(Rectangle rect)
        {
            return ds.CreateLayer(1, rect.ToWindowsRect());
        }
    }
}