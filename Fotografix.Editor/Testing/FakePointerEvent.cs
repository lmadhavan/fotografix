using System.Drawing;

namespace Fotografix.Editor.Testing
{
    public class FakePointerEvent : IPointerEvent
    {
        public FakePointerEvent()
        {
        }

        public FakePointerEvent(int x, int y) : this(new Point(x, y))
        {
        }

        public FakePointerEvent(Point location)
        {
            this.Location = location;
            this.ViewportLocation = location;
        }

        public Point Location { get; set; }
        public PointF ViewportLocation { get; set; }
    }
}
