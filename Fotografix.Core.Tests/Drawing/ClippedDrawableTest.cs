using Moq;
using NUnit.Framework;
using System;
using System.Drawing;

namespace Fotografix.Drawing
{
    [TestFixture]
    public class ClippedDrawableTest
    {
        private static readonly Rectangle DrawableBounds = Rectangle.FromLTRB(0, 0, 50, 50);
        private static readonly Rectangle ClipRectangle = Rectangle.FromLTRB(20, 20, 100, 100);

        private Mock<IDrawable> drawable;
        private Mock<IDrawingContext> drawingContext;
        private Mock<IDisposable> clipContext;

        [SetUp]
        public void SetUp()
        {
            this.drawable = new Mock<IDrawable>();
            drawable.SetupGet(d => d.Bounds).Returns(DrawableBounds);

            this.drawingContext = new Mock<IDrawingContext>();
            this.clipContext = new Mock<IDisposable>();
            drawingContext.Setup(dc => dc.BeginClip(It.IsAny<Rectangle>())).Returns(clipContext.Object);
        }

        [Test]
        public void ComputesClippedBounds()
        {
            IDrawable clippedDrawable = ClippedDrawable.Create(drawable.Object, ClipRectangle);

            Assert.That(clippedDrawable.Bounds, Is.EqualTo(Rectangle.Intersect(DrawableBounds, ClipRectangle)));
        }

        [Test]
        public void ClipsDrawingToClipRectangle()
        {
            IDrawable clippedDrawable = ClippedDrawable.Create(drawable.Object, ClipRectangle);

            clippedDrawable.Draw(drawingContext.Object);

            drawingContext.Verify(dc => dc.BeginClip(ClipRectangle));
            drawable.Verify(d => d.Draw(drawingContext.Object));
            clipContext.Verify(cc => cc.Dispose());
        }

        [Test]
        public void PassesThroughChangeEventsFromWrappedDrawable()
        {
            bool changed = false;

            IDrawable clippedDrawable = ClippedDrawable.Create(drawable.Object, ClipRectangle);
            clippedDrawable.Changed += (s, e) => changed = true;

            drawable.Raise(d => d.Changed += null, EventArgs.Empty);

            Assert.IsTrue(changed);
        }

        [Test]
        public void DoesNotWrapDrawableWhenClipRectangleIsEmpty()
        {
            IDrawable clippedDrawable = ClippedDrawable.Create(drawable.Object, Rectangle.Empty);
            Assert.That(clippedDrawable, Is.SameAs(drawable.Object));
        }
    }
}
