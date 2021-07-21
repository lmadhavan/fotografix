using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Drawing
{
    [TestFixture]
    public class ClippedDrawableTest
    {
        private static readonly Rectangle DrawableBounds = Rectangle.FromLTRB(0, 0, 50, 50);
        private static readonly Rectangle ClipRectangle = Rectangle.FromLTRB(20, 20, 100, 100);

        private TestDrawable drawable;
        private TestGraphicsDevice device;

        [SetUp]
        public void SetUp()
        {
            this.drawable = new(DrawableBounds);
            this.device = new();
        }

        [Test]
        public void ComputesClippedBounds()
        {
            IDrawable clippedDrawable = ClippedDrawable.Create(drawable, ClipRectangle);

            Assert.That(clippedDrawable.Bounds, Is.EqualTo(Rectangle.Intersect(DrawableBounds, ClipRectangle)));
        }

        [Test]
        public void ClipsDrawingToClipRectangle()
        {
            IDrawable clippedDrawable = ClippedDrawable.Create(drawable, ClipRectangle);

            clippedDrawable.Draw(device);

            device.VerifySequence(
                ("BeginClip", ClipRectangle),
                ("Draw", drawable),
                ("EndClip")
            );
        }

        [Test]
        public void PassesThroughChangeEventsFromWrappedDrawable()
        {
            bool changed = false;

            IDrawable clippedDrawable = ClippedDrawable.Create(drawable, ClipRectangle);
            clippedDrawable.Changed += (s, e) => changed = true;

            drawable.RaiseChanged();

            Assert.IsTrue(changed);
        }

        [Test]
        public void DoesNotWrapDrawableWhenClipRectangleIsEmpty()
        {
            IDrawable clippedDrawable = ClippedDrawable.Create(drawable, Rectangle.Empty);
            Assert.That(clippedDrawable, Is.SameAs(drawable));
        }
    }
}
