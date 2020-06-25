using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor
{
    [TestFixture]
    public class ViewportTest
    {
        [Test]
        public void ZoomsOutToFitLargeWidth()
        {
            Viewport viewport = new FakeViewport(100, 100);

            viewport.ZoomToFit(new Size(200, 50));

            Assert.That(viewport.ZoomFactor, Is.EqualTo(0.5f));
        }

        [Test]
        public void ZoomsOutToFitLargeHeight()
        {
            Viewport viewport = new FakeViewport(100, 100);

            viewport.ZoomToFit(new Size(50, 200));

            Assert.That(viewport.ZoomFactor, Is.EqualTo(0.5f));
        }

        [Test]
        public void DoesNotZoomWhenContentFitsEntirelyWithinViewport()
        {
            Viewport viewport = new FakeViewport(100, 100);

            viewport.ZoomToFit(new Size(50, 50));

            Assert.That(viewport.ZoomFactor, Is.EqualTo(1.0f));
        }

        [Test]
        public void AdjustsScrollOffsetForContentBasedOnZoomFactor()
        {
            Viewport viewport = new FakeViewport(100, 100)
            {
                ZoomFactor = 2,
                ScrollOffset = new PointF(10, 10)
            };

            viewport.ScrollContentBy(new PointF(3, 3));

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new PointF(16, 16)));
        }

        private sealed class FakeViewport : Viewport
        {
            public FakeViewport(int width, int height)
            {
                this.Width = width;
                this.Height = height;
            }

            public override int Width { get; }
            public override int Height { get; }

            public override float ZoomFactor { get; set; }
            public override PointF ScrollOffset { get; set; }
        }
    }
}
