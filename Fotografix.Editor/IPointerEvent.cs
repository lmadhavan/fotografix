using System.Drawing;

namespace Fotografix.Editor
{
    public interface IPointerEvent
    {
        /// <summary>
        /// Gets the location of the pointer in image coordinates.
        /// </summary>
        Point Location { get; }

        /// <summary>
        /// Gets the location of the pointer in viewport coordinates.
        /// </summary>
        PointF ViewportLocation { get; }
    }
}
