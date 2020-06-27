using Fotografix.Editor.Testing;
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
    }
}
