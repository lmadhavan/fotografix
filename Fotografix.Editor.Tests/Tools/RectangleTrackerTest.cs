using NUnit.Framework;
using System.Drawing;
using System.Linq;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class RectangleTrackerTest
    {
        private RectangleTracker tracker;

        [SetUp]
        public void SetUp()
        {
            this.tracker = new RectangleTracker(new Rectangle(0, 0, 100, 100));
        }

        [Test]
        public void TracksTopLeft()
        {
            Drag(from: new Point(0, 0), to: new Point(10, 10));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNorthwestSoutheast));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(10, 10, 100, 100)));
        }

        [Test]
        public void TracksTop()
        {
            Drag(from: new Point(50, 0), to: new Point(60, 10));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNorthSouth));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(0, 10, 100, 100)));
        }

        [Test]
        public void TracksTopRight()
        {
            Drag(from: new Point(100, 0), to: new Point(90, 10));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNortheastSouthwest));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(0, 10, 90, 100)));
        }

        [Test]
        public void TracksLeft()
        {
            Drag(from: new Point(0, 50), to: new Point(10, 60));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeWestEast));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(10, 0, 100, 100)));
        }

        [Test]
        public void TracksRight()
        {
            Drag(from: new Point(100, 50), to: new Point(90, 60));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeWestEast));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(0, 0, 90, 100)));
        }

        [Test]
        public void TracksBottomLeft()
        {
            Drag(from: new Point(0, 100), to: new Point(10, 90));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNortheastSouthwest));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(10, 0, 100, 90)));
        }

        [Test]
        public void TracksBottom()
        {
            Drag(from: new Point(50, 100), to: new Point(60, 90));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNorthSouth));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(0, 0, 100, 90)));
        }

        [Test]
        public void TracksBottomRight()
        {
            Drag(from: new Point(100, 100), to: new Point(90, 90));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNorthwestSoutheast));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(0, 0, 90, 90)));
        }

        [Test]
        public void TracksCenter()
        {
            Drag(from: new Point(50, 50), to: new Point(60, 60));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.Move));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(10, 10, 110, 110)));
        }

        [Test]
        public void TracksMultiplePointsInSingleDragOperation()
        {
            Drag(
                new Point(0, 0),
                new Point(5, 5),
                new Point(10, 10)
            );

            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(10, 10, 100, 100)));
        }

        [Test]
        public void DisabledOutside()
        {
            Drag(from: new Point(150, 150), to: new Point(50, 50));

            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.Disabled));
            Assert.That(tracker.Rectangle, Is.EqualTo(Rectangle.FromLTRB(0, 0, 100, 100)));
        }

        [Test]
        public void AccountsForHandleTolerance()
        {
            tracker.HandleTolerance = 5;

            Move(new Point(5, 0));
            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNorthwestSoutheast));

            Move(new Point(6, 0));
            Assert.That(tracker.Cursor, Is.EqualTo(ToolCursor.SizeNorthSouth));
        }

        private void Drag(Point from, Point to)
        {
            Drag(start: from, points: to);
        }

        private void Drag(Point start, params Point[] points)
        {
            tracker.PointerPressed(new PointerState(start));
            foreach (Point pt in points)
            {
                tracker.PointerMoved(new PointerState(pt));
            }
            tracker.PointerReleased(new PointerState(points.Last()));
        }

        private void Move(Point to)
        {
            tracker.PointerMoved(new PointerState(to));
        }
    }
}
