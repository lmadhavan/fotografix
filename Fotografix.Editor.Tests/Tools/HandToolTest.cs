using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class HandToolTest
    {
        private HandTool tool;
        private Document document;
        private Viewport viewport;

        [SetUp]
        public void SetUp()
        {
            this.tool = new HandTool();

            this.document = new Document();
            this.viewport = document.Viewport;
            viewport.Size = new Size(100, 100);
            viewport.ImageSize = new Size(200, 200);
        }

        [Test]
        public void TranslatesPointerDragIntoViewportScroll()
        {
            tool.Activated(document);
            viewport.ScrollOffset = new Point(10, 10);

            tool.PointerPressed(ViewportPoint(30, 30));
            tool.PointerMoved(ViewportPoint(32, 32));

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new Point(8, 8)));
        }

        [Test]
        public void IgnoresMovementWhenPointerNotPressed()
        {
            tool.Activated(document);
            viewport.ScrollOffset = new Point(10, 10);

            tool.PointerMoved(ViewportPoint(20, 20));

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new Point(10, 10)));
        }

        [Test]
        public void IgnoresMovementAfterPointerReleased()
        {
            tool.Activated(document);
            tool.PointerPressed(ViewportPoint(10, 10));
            tool.PointerMoved(ViewportPoint(20, 20));
            tool.PointerReleased(ViewportPoint(30, 30));

            viewport.ScrollOffset = new Point(10, 10);

            tool.PointerMoved(ViewportPoint(40, 40));

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new Point(10, 10)));
        }

        private PointerState ViewportPoint(int x, int y)
        {
            return viewport.TransformViewportToImage(new Point(x, y));
        }
    }
}
