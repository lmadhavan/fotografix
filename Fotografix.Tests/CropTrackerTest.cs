using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Core;
using static Fotografix.CropTracker;

namespace Fotografix
{
    [TestClass]
    public class CropTrackerTest
    {
        private CropTracker tracker;

        [TestInitialize]
        public void Initialize()
        {
            this.tracker = new CropTracker { Rect = new Rect(0, 0, 100, 100) };
        }

        [TestMethod]
        public void TracksTopLeft()
        {
            Drag(from: new Point(0, 0), to: new Point(10, 10));

            Assert.AreEqual(CoreCursorType.SizeNorthwestSoutheast, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(10, 10, 100, 100), tracker.Rect);
        }

        [TestMethod]
        public void TracksTop()
        {
            Drag(from: new Point(50, 0), to: new Point(60, 10));

            Assert.AreEqual(CoreCursorType.SizeNorthSouth, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(0, 10, 100, 100), tracker.Rect);
        }

        [TestMethod]
        public void TracksTopRight()
        {
            Drag(from: new Point(100, 0), to: new Point(90, 10));

            Assert.AreEqual(CoreCursorType.SizeNortheastSouthwest, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(0, 10, 90, 100), tracker.Rect);
        }

        [TestMethod]
        public void TracksLeft()
        {
            Drag(from: new Point(0, 50), to: new Point(10, 60));

            Assert.AreEqual(CoreCursorType.SizeWestEast, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(10, 0, 100, 100), tracker.Rect);
        }

        [TestMethod]
        public void TracksRight()
        {
            Drag(from: new Point(100, 50), to: new Point(90, 60));

            Assert.AreEqual(CoreCursorType.SizeWestEast, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(0, 0, 90, 100), tracker.Rect);
        }

        [TestMethod]
        public void TracksBottomLeft()
        {
            Drag(from: new Point(0, 100), to: new Point(10, 90));

            Assert.AreEqual(CoreCursorType.SizeNortheastSouthwest, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(10, 0, 100, 90), tracker.Rect);
        }

        [TestMethod]
        public void TracksBottom()
        {
            Drag(from: new Point(50, 100), to: new Point(60, 90));

            Assert.AreEqual(CoreCursorType.SizeNorthSouth, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(0, 0, 100, 90), tracker.Rect);
        }

        [TestMethod]
        public void TracksBottomRight()
        {
            Drag(from: new Point(100, 100), to: new Point(90, 90));

            Assert.AreEqual(CoreCursorType.SizeNorthwestSoutheast, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(0, 0, 90, 90), tracker.Rect);
        }

        [TestMethod]
        public void TracksCenter()
        {
            Drag(from: new Point(50, 50), to: new Point(60, 60));

            Assert.AreEqual(CoreCursorType.SizeAll, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(10, 10, 110, 110), tracker.Rect);
        }

        [TestMethod]
        public void TracksMultiplePointsInSingleDragOperation()
        {
            Drag(
                new Point(0, 0),
                new Point(5, 5),
                new Point(10, 10)
            );

            Assert.AreEqual(RectFromLTRB(10, 10, 100, 100), tracker.Rect);
        }

        [TestMethod]
        public void DisabledOutside()
        {
            Drag(from: new Point(150, 150), to: new Point(50, 50));

            Assert.AreEqual(CoreCursorType.UniversalNo, tracker.Cursor.Type);
            Assert.AreEqual(RectFromLTRB(0, 0, 100, 100), tracker.Rect);
        }

        [TestMethod]
        public void AccountsForHandleTolerance()
        {
            tracker.HandleTolerance = 5;

            Move(new Point(5, 0));
            Assert.AreEqual(CoreCursorType.SizeNorthwestSoutheast, tracker.Cursor.Type);

            Move(new Point(6, 0));
            Assert.AreEqual(CoreCursorType.SizeNorthSouth, tracker.Cursor.Type);
        }

        private void Drag(Point from, Point to)
        {
            Drag(start: from, points: to);
        }

        private void Drag(Point start, params Point[] points)
        {
            tracker.PointerPressed(start);
            foreach (Point pt in points)
            {
                tracker.PointerMoved(pt);
            }
            tracker.PointerReleased(points.Last());
        }

        private void Move(Point to)
        {
            tracker.PointerMoved(to);
        }
    }
}
