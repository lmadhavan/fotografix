using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class HandToolTest
    {
        private HandTool tool;
        private ViewportScrollRequestedEventArgs scrollEvent;

        [SetUp]
        public void SetUp()
        {
            this.tool = new HandTool();
            tool.ViewportScrollRequested += (s, e) => this.scrollEvent = e;
            this.scrollEvent = null;
        }

        [Test]
        public void TranslatesPointerDragIntoScrollRequest()
        {
            tool.PointerPressed(new PointF(10, 10));
            tool.PointerMoved(new PointF(20, 20));

            Assert.That(scrollEvent.ScrollDelta, Is.EqualTo(new PointF(-10, -10)));
        }

        [Test]
        public void ComputesDeltaFromLastPoint()
        {
            tool.PointerPressed(new PointF(10, 10));
            tool.PointerMoved(new PointF(20, 20));
            tool.PointerMoved(new PointF(25, 25));

            Assert.That(scrollEvent.ScrollDelta, Is.EqualTo(new PointF(-5, -5)));
        }

        [Test]
        public void IgnoresMovementWhenPointerNotPressed()
        {
            tool.PointerMoved(new PointF(10, 10));

            Assert.That(scrollEvent, Is.Null);
        }

        [Test]
        public void IgnoresMovementAfterPointerReleased()
        {
            tool.PointerPressed(new PointF(10, 10));
            tool.PointerMoved(new PointF(20, 20));
            tool.PointerReleased(new PointF(30, 30));

            this.scrollEvent = null;
            tool.PointerMoved(new PointF(40, 40));

            Assert.That(scrollEvent, Is.Null);
        }
    }
}
