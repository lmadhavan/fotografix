using System.Drawing;

namespace Fotografix.Editor
{
    public sealed class PointerState
    {
        public static readonly PointerState Empty = new PointerState(Point.Empty);

        public PointerState(Point location) : this(location, location)
        {
        }

        public PointerState(Point location, PointF viewportLocation)
        {
            this.Location = location;
            this.ViewportLocation = viewportLocation;
        }

        /// <summary>
        /// Gets the location of the pointer in image coordinates.
        /// </summary>
        public Point Location { get; }

        /// <summary>
        /// Gets the location of the pointer in viewport coordinates.
        /// </summary>
        public PointF ViewportLocation { get; }

        public static implicit operator PointerState(Point pt)
        {
            return new PointerState(pt);
        }
    }
}
