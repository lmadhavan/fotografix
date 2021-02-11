using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Drawing
{
    public sealed class BrushStroke : IDrawable
    {
        private readonly List<Point> points;

        public BrushStroke(Point start, int size, Color color) : this(new Point[] { start }, size, color)
        {
        }

        public BrushStroke(IEnumerable<Point> points, int size, Color color)
        {
            this.points = new List<Point>(points);
            this.Size = size;
            this.Color = color;
        }

        public IReadOnlyList<Point> Points => points;
        public int Size { get; }
        public Color Color { get; }

        public event EventHandler Changed;

        public void AddPoint(Point pt)
        {
            points.Add(pt);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(IDrawingContext drawingContext)
        {
            drawingContext.Draw(this);
        }
    }
}
