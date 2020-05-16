using System.Collections.Generic;
using System.Drawing;

namespace Fotografix
{
    public sealed class BrushStroke
    {
        private readonly List<PointF> points = new List<PointF>();

        public BrushStroke(PointF pt, float size, Color color)
        {
            points.Add(pt);
            this.Size = size;
            this.Color = color;
        }

        public float Size { get; }
        public Color Color { get; }

        public IReadOnlyList<PointF> Points => points;

        public void AddPoint(PointF pt)
        {
            points.Add(pt);
        }
    }
}
