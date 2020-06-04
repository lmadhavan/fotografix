using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tests
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
        }
    }
}
