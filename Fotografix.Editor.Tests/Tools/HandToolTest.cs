using Fotografix.Editor.Testing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class HandToolTest
    {
        private Viewport viewport;
        private HandTool tool;

        [SetUp]
        public void SetUp()
        {
            this.viewport = new FakeViewport(100, 100);
            this.tool = new HandTool(viewport);
        }

        [Test]
        public void TranslatesPointerDragIntoViewportScroll()
        {
            viewport.ScrollOffset = new PointF(10, 10);

            tool.PointerPressed(ViewportLocation(30, 30));
            tool.PointerMoved(ViewportLocation(32, 32));

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new PointF(8, 8)));
        }

        [Test]
        public void IgnoresMovementWhenPointerNotPressed()
        {
            viewport.ScrollOffset = new PointF(10, 10);

            tool.PointerMoved(ViewportLocation(20, 20));

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new PointF(10, 10)));
        }

        [Test]
        public void IgnoresMovementAfterPointerReleased()
        {
            tool.PointerPressed(ViewportLocation(10, 10));
            tool.PointerMoved(ViewportLocation(20, 20));
            tool.PointerReleased(ViewportLocation(30, 30));

            viewport.ScrollOffset = new PointF(10, 10);

            tool.PointerMoved(ViewportLocation(40, 40));

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new PointF(10, 10)));
        }

        private PointerState ViewportLocation(float x, float y)
        {
            return new PointerState(
                location: Point.Empty,
                viewportLocation: new PointF(x, y)
            );
        }
    }
}
