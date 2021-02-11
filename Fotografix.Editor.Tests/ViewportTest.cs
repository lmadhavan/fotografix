using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor
{
    [TestFixture]
    public class ViewportTest
    {
        [Test]
        public void TransformsViewportPointToImagePoint()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(125, 125),
                ZoomFactor = 2,
                ScrollOffset = new Point(10, 10)
            };

            Assert.That(viewport.TransformViewportToImage(new Point(20, 20)), Is.EqualTo(new Point(15, 15)));
        }

        [Test]
        public void TransformsImagePointToViewportPoint()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(125, 125),
                ZoomFactor = 2,
                ScrollOffset = new Point(10, 10)
            };

            Assert.That(viewport.TransformImageToViewport(new Point(20, 20)), Is.EqualTo(new Point(30, 30)));
        }

        [Test]
        public void DisallowsScrollingWhenImageIsSmallerThanViewport()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(200, 200),
                ImageSize = new Size(100, 100),
                ScrollOffset = new Point(500, 500)
            };

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new Point(0, 0)));
        }

        [Test]
        public void EnforcesMinimumScrollOffset()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(125, 125),
                ZoomFactor = 2,
                ScrollOffset = new Point(-5, -5)
            };

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new Point(0, 0)));
        }

        [Test]
        public void EnforcesMaximumScrollOffsetBasedOnRenderedImageSize()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(125, 125),
                ZoomFactor = 2,
                ScrollOffset = new Point(200, 200)
            };

            Assert.That(viewport.ScrollOffset, Is.EqualTo(new Point(150, 150)));
        }

        [Test]
        public void CentersImageWhenSmallerThanViewport()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(20, 20),
                ZoomFactor = 2
            };

            Assert.That(viewport.TransformImageToViewport(new Point(0, 0)), Is.EqualTo(new Point(30, 30)));
        }

        [Test]
        public void ZoomsOutToFitLargeWidth()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(200, 50)
            };

            viewport.ZoomToFit();

            Assert.That(viewport.ZoomFactor, Is.EqualTo(0.5f));
        }

        [Test]
        public void ZoomsOutToFitLargeHeight()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(50, 200)
            };

            viewport.ZoomToFit();

            Assert.That(viewport.ZoomFactor, Is.EqualTo(0.5f));
        }

        [Test]
        public void ZoomToFitDoesNothingWhenImageIsSmallerThanViewport()
        {
            Viewport viewport = new Viewport
            {
                Size = new Size(100, 100),
                ImageSize = new Size(50, 50)
            };

            viewport.ZoomToFit();

            Assert.That(viewport.ZoomFactor, Is.EqualTo(1.0f));
        }
    }
}
