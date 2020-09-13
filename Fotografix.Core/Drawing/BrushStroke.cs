using System.Collections.Generic;
using System.Drawing;

namespace Fotografix.Drawing
{
    public sealed class BrushStroke : NotifyContentChangedBase, IDrawable
    {
        private readonly List<Point> points;

        public BrushStroke(Point start, int size, Color color)
        {
            this.points = new List<Point> { start };
            this.Size = size;
            this.Color = color;
        }

        public IReadOnlyList<Point> Points => points;
        public int Size { get; }
        public Color Color { get; }

        public void AddPoint(Point pt)
        {
            points.Add(pt);
            RaiseContentChanged();
        }

        public void Draw(IDrawingContext drawingContext)
        {
            drawingContext.Draw(this);
        }
    }
}
