using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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

        public sealed class TestCase
        {
            public string Name { get; set; }

            public Point Start { get; set; }
            public Point End { get; set; }

            public CoreCursorType ExpectedCursor { get; set; }
            public Rect ExpectedRect { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(CropHandleTestData), DynamicDataSourceType.Method)]
        public void TracksHandle(TestCase tc)
        {
            Drag(tc.Start, tc.End);

            Assert.AreEqual(tc.ExpectedCursor, tracker.Cursor.Type);
            Assert.AreEqual(tc.ExpectedRect, tracker.Rect);
        }

        private static IEnumerable<object[]> CropHandleTestData()
        {
            object[] TestCase(string name, Point start, Point end, CoreCursorType expectedCursor, Rect expectedRect)
            {
                return new object[] { new TestCase {
                    Name = name,
                    Start = start,
                    End = end,
                    ExpectedCursor = expectedCursor,
                    ExpectedRect = expectedRect
                } };
            }

            yield return TestCase("TopLeft",     new Point(0, 0),     new Point(10, 10), CoreCursorType.SizeNorthwestSoutheast, RectFromLTRB(10, 10, 100, 100));
            yield return TestCase("Top",         new Point(50, 0),    new Point(60, 10), CoreCursorType.SizeNorthSouth,         RectFromLTRB(0, 10, 100, 100));
            yield return TestCase("TopRight",    new Point(100, 0),   new Point(90, 10), CoreCursorType.SizeNortheastSouthwest, RectFromLTRB(0, 10, 90, 100));
            yield return TestCase("Left",        new Point(0, 50),    new Point(10, 60), CoreCursorType.SizeWestEast,           RectFromLTRB(10, 0, 100, 100));
            yield return TestCase("Right",       new Point(100, 50),  new Point(90, 60), CoreCursorType.SizeWestEast,           RectFromLTRB(0, 0, 90, 100));
            yield return TestCase("BottomLeft",  new Point(0, 100),   new Point(10, 90), CoreCursorType.SizeNortheastSouthwest, RectFromLTRB(10, 0, 100, 90));
            yield return TestCase("Bottom",      new Point(50, 100),  new Point(60, 90), CoreCursorType.SizeNorthSouth,         RectFromLTRB(0, 0, 100, 90));
            yield return TestCase("BottomRight", new Point(100, 100), new Point(90, 90), CoreCursorType.SizeNorthwestSoutheast, RectFromLTRB(0, 0, 90, 90));
            yield return TestCase("Center",      new Point(50, 50),   new Point(60, 60), CoreCursorType.SizeAll,                RectFromLTRB(10, 10, 110, 110));
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

        [TestMethod]
        public void LimitsSizeToMaxBounds()
        {
            tracker.MaxBounds = new Rect(0, 0, 200, 150);

            Drag(from: new Point(100, 100), to: new Point(300, 300));

            Assert.AreEqual(tracker.MaxBounds, tracker.Rect);
        }

        [TestMethod]
        public void EnsuresMinSize()
        {
            tracker.MinSize = new Size(10, 5);

            Drag(from: new Point(100, 100), to: new Point(-10, -10));

            Assert.AreEqual(new Rect(0, 0, 10, 5), tracker.Rect);
        }

        [TestMethod]
        public void PreventsMoveOutsideMaxBounds()
        {
            tracker.MaxBounds = new Rect(0, 0, 200, 150);

            Drag(from: new Point(50, 50), to: new Point(300, 50));

            Assert.AreEqual(new Rect(100, 0, 100, 100), tracker.Rect);
        }

        [DataTestMethod]
        [DynamicData(nameof(AspectRatioTestData), DynamicDataSourceType.Method)]
        public void MaintainsAspectRatio(TestCase tc)
        {
            tracker.Rect = new Rect(0, 0, 100, 50);
            tracker.AspectRatio = 2f;

            Drag(tc.Start, tc.End);

            Assert.AreEqual(tc.ExpectedRect, tracker.Rect);
        }

        private static IEnumerable<object[]> AspectRatioTestData()
        {
            object[] TestCase(string name, Point start, Point end, Rect expectedRect)
            {
                return new object[] { new TestCase {
                    Name = name,
                    Start = start,
                    End = end,
                    ExpectedRect = expectedRect
                } };
            }

            yield return TestCase("TopLeft",     new Point(0, 0),     new Point(10, 10), new Rect(20, 10, 80, 40));
            yield return TestCase("Top",         new Point(50, 0),    new Point(50, 10), new Rect(0, 10, 80, 40));
            yield return TestCase("TopRight",    new Point(100, 0),   new Point(90, 10), new Rect(0, 10, 80, 40));
            yield return TestCase("Left",        new Point(0, 25),    new Point(10, 25), new Rect(10, 0, 90, 45));
            yield return TestCase("Right",       new Point(100, 25),  new Point(90, 25), new Rect(0, 0, 90, 45));
            yield return TestCase("BottomLeft",  new Point(0, 50),    new Point(10, 40), new Rect(20, 0, 80, 40));
            yield return TestCase("Bottom",      new Point(50, 50),   new Point(50, 40), new Rect(0, 0, 80, 40));
            yield return TestCase("BottomRight", new Point(100, 50),  new Point(90, 40), new Rect(0, 0, 80, 40));
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
