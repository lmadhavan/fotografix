using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Fotografix.Drawing
{
    public sealed class BrushStroke : IDrawable
    {
        private readonly List<Point> points;

        public BrushStroke(Point start, int size, Color color)
        {
            this.points = new List<Point> { start };

            this.Size = size;
            this.Color = color;
            this.Bounds = BoundingBox(start);
        }

        public BrushStroke(IEnumerable<Point> points, int size, Color color) : this(points.First(), size, color)
        {
            foreach (Point pt in points.Skip(1))
            {
                AddPoint(pt);
            }
        }

        public IReadOnlyList<Point> Points => points;
        public Rectangle Bounds { get; private set; }

        public int Size { get; }
        public Color Color { get; }

        public event EventHandler Changed;

        public void AddPoint(Point pt)
        {
            points.Add(pt);
            this.Bounds = Rectangle.Union(Bounds, BoundingBox(pt));

            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(IDrawingContext drawingContext)
        {
            drawingContext.Draw(this);
        }

        private Rectangle BoundingBox(Point pt)
        {
            return Rectangle.FromLTRB(
                (int)Math.Floor(pt.X - Size / 2f),
                (int)Math.Floor(pt.Y - Size / 2f),
                (int)Math.Ceiling(pt.X + Size / 2f),
                (int)Math.Ceiling(pt.Y + Size / 2f)
            );
        }
    }
}
