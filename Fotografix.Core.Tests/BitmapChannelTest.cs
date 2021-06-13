using Fotografix.Drawing;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix
{
    [TestFixture]
    public class BitmapChannelTest
    {
        private Mock<IDrawingContextFactory> drawingContextFactory;
        private Mock<IDrawingContext> drawingContext;
        private Mock<IDrawable> drawable;

        [SetUp]
        public void SetUp()
        {
            this.drawingContextFactory = new Mock<IDrawingContextFactory>();
            this.drawingContext = new Mock<IDrawingContext>(MockBehavior.Strict);
            this.drawable = new Mock<IDrawable>(MockBehavior.Strict);

            drawingContextFactory.Setup(f => f.CreateDrawingContext(It.IsAny<Bitmap>())).Returns(drawingContext.Object);
            drawingContext.Setup(dc => dc.Dispose());
        }

        [Test]
        public void DrawsDrawableOnBitmap()
        {
            Bitmap bitmap = new Bitmap(new Size(10, 10));

            drawable.SetupGet(d => d.Bounds).Returns(bitmap.Bounds);
            drawable.Setup(d => d.Draw(It.IsAny<IDrawingContext>()));

            BitmapChannel channel = new BitmapChannel(bitmap);
            channel.Draw(drawable.Object, drawingContextFactory.Object);

            drawingContextFactory.Verify(f => f.CreateDrawingContext(bitmap));
            drawable.Verify();
        }

        [Test]
        public void ExpandsBitmapToAccommodateDrawable()
        {
            Bitmap bitmap = new Bitmap(new Rectangle(10, 10, 20, 20));

            drawable.SetupGet(d => d.Bounds).Returns(new Rectangle(5, 5, 10, 10));

            MockSequence sequence = new MockSequence();
            drawingContext.InSequence(sequence).Setup(dc => dc.Draw(It.IsAny<Bitmap>(), It.IsAny<Rectangle>(), It.IsAny<Rectangle>()));
            drawable.InSequence(sequence).Setup(d => d.Draw(It.IsAny<IDrawingContext>()));

            BitmapChannel channel = new BitmapChannel(bitmap);
            channel.Draw(drawable.Object, drawingContextFactory.Object);

            Bitmap newBitmap = channel.Bitmap;
            Assert.That(newBitmap.Bounds, Is.EqualTo(Rectangle.FromLTRB(5, 5, 30, 30)));

            drawingContextFactory.Verify(f => f.CreateDrawingContext(newBitmap));
            drawingContext.Verify(dc => dc.Draw(bitmap, bitmap.Bounds, bitmap.Bounds));
            drawable.Verify(d => d.Draw(drawingContext.Object));
        }

        [Test]
        public void IgnoresExistingBitmapIfEmpty()
        {
            Bitmap bitmap = new Bitmap(Size.Empty);

            Rectangle drawableBounds = new Rectangle(5, 5, 10, 10);
            drawable.SetupGet(d => d.Bounds).Returns(drawableBounds);
            drawable.Setup(d => d.Draw(It.IsAny<IDrawingContext>()));

            BitmapChannel channel = new BitmapChannel(bitmap);
            channel.Draw(drawable.Object, drawingContextFactory.Object);

            Bitmap newBitmap = channel.Bitmap;
            Assert.That(newBitmap.Bounds, Is.EqualTo(drawableBounds));

            drawingContextFactory.Verify(f => f.CreateDrawingContext(newBitmap));
            drawable.Verify(d => d.Draw(drawingContext.Object));
        }

        [Test]
        public void ScalesBitmapBySpecifiedFactor()
        {
            Bitmap bitmap = new Bitmap(new Rectangle(10, 10, 20, 20));
            BitmapChannel channel = new BitmapChannel(bitmap);

            drawingContext.Setup(dc => dc.Draw(It.IsAny<Bitmap>(), It.IsAny<Rectangle>(), It.IsAny<Rectangle>()));

            channel.Scale(new(1.5f, 1.5f), drawingContextFactory.Object);

            Bitmap newBitmap = channel.Bitmap;
            Assert.That(newBitmap.Bounds, Is.EqualTo(new Rectangle(15, 15, 30, 30)));

            drawingContextFactory.Verify(f => f.CreateDrawingContext(newBitmap));
            drawingContext.Verify(dc => dc.Draw(bitmap, newBitmap.Bounds, bitmap.Bounds));
        }
    }
}
