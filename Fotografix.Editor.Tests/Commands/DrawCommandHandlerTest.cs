using Fotografix.Drawing;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Commands
{
    [TestFixture]
    public class DrawCommandHandlerTest
    {
        private Mock<IDrawingContextFactory> drawingContextFactory;
        private Mock<IDrawingContext> drawingContext;
        private Mock<IDrawable> drawable;

        private DrawCommandHandler handler;

        [SetUp]
        public void SetUp()
        {
            this.drawingContextFactory = new Mock<IDrawingContextFactory>();
            this.drawingContext = new Mock<IDrawingContext>(MockBehavior.Strict);
            this.drawable = new Mock<IDrawable>(MockBehavior.Strict);

            drawingContextFactory.Setup(f => f.CreateDrawingContext(It.IsAny<Bitmap>())).Returns(drawingContext.Object);
            drawingContext.Setup(dc => dc.Dispose());
            
            this.handler = new DrawCommandHandler(drawingContextFactory.Object);
        }

        [Test]
        public void DrawsDrawableOnBitmap()
        {
            Bitmap bitmap = new Bitmap(new Size(10, 10));
            Layer layer = new Layer(bitmap);

            drawable.SetupGet(d => d.Bounds).Returns(bitmap.Bounds);
            drawable.Setup(d => d.Draw(It.IsAny<IDrawingContext>()));

            handler.Handle(new DrawCommand(layer, drawable.Object));

            drawingContextFactory.Verify(f => f.CreateDrawingContext(bitmap));
            drawable.Verify();
        }

        [Test]
        public void ExpandsBitmapToAccommodateDrawable()
        {
            Bitmap bitmap = new Bitmap(new Rectangle(10, 10, 20, 20));
            Layer layer = new Layer(bitmap);

            drawable.SetupGet(d => d.Bounds).Returns(new Rectangle(5, 5, 10, 10));

            MockSequence sequence = new MockSequence();
            drawingContext.InSequence(sequence).Setup(dc => dc.Draw(It.IsAny<Bitmap>()));
            drawable.InSequence(sequence).Setup(d => d.Draw(It.IsAny<IDrawingContext>()));

            handler.Handle(new DrawCommand(layer, drawable.Object));

            Bitmap newBitmap = (Bitmap)layer.Content;
            Assert.That(newBitmap.Bounds, Is.EqualTo(Rectangle.FromLTRB(5, 5, 30, 30)));

            drawingContextFactory.Verify(f => f.CreateDrawingContext(newBitmap));
            drawingContext.Verify(dc => dc.Draw(bitmap));
            drawable.Verify(d => d.Draw(drawingContext.Object));
        }

        [Test]
        public void IgnoresExistingBitmapIfEmpty()
        {
            Bitmap bitmap = new Bitmap(Size.Empty);
            Layer layer = new Layer(bitmap);

            Rectangle drawableBounds = new Rectangle(5, 5, 10, 10);
            drawable.SetupGet(d => d.Bounds).Returns(drawableBounds);
            drawable.Setup(d => d.Draw(It.IsAny<IDrawingContext>()));

            handler.Handle(new DrawCommand(layer, drawable.Object));

            Bitmap newBitmap = (Bitmap)layer.Content;
            Assert.That(newBitmap.Bounds, Is.EqualTo(drawableBounds));

            drawingContextFactory.Verify(f => f.CreateDrawingContext(newBitmap));
            drawable.Verify(d => d.Draw(drawingContext.Object));
        }
    }
}
