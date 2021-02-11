using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class PointerState
    {
        public static readonly PointerState Empty = new PointerState(Point.Empty);

        public PointerState(Point location)
        {
            this.Location = location;
        }

        public Point Location { get; }

        public static implicit operator PointerState(Point pt)
        {
            return new PointerState(pt);
        }
    }
}
