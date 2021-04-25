using Fotografix.Drawing;
using Fotografix.Editor.Commands;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class DrawingToolTest : BitmapToolTest
    {
        private static readonly PointerState Start = new(10, 10);
        private static readonly PointerState End = new(20, 20);

        private ITool tool;
        
        private Mock<IFakeDrawableFactory> drawableFactory;
        private Mock<IFakeDrawable> drawable;
        private Mock<ICommandDispatcher> commandDispatcher;

        protected override ITool Tool => tool;

        [SetUp]
        public void SetUp()
        {
            this.drawableFactory = new Mock<IFakeDrawableFactory>();
            this.drawable = new Mock<IFakeDrawable>();
            drawableFactory.Setup(f => f.Create(It.IsAny<Image>(), It.IsAny<Point>())).Returns(drawable.Object);

            this.commandDispatcher = new Mock<ICommandDispatcher>();
            Image.SetCommandDispatcher(commandDispatcher.Object);

            this.tool = new FakeDrawingTool(drawableFactory.Object);
        }

        [Test]
        public void BeginsDrawingPreviewWhenPointerPressed()
        {
            Activate(BitmapLayer);

            tool.PointerPressed(Start);

            drawableFactory.Verify(f => f.Create(Image, Start.Location));
            Assert.That(Bitmap.GetDrawingPreview(), Is.EqualTo(drawable.Object));
        }

        [Test]
        public void UpdatesDrawingWhenPointerMoved()
        {
            Activate(BitmapLayer);

            tool.PointerPressed(Start);
            tool.PointerMoved(End);

            drawable.Verify(d => d.Update(End.Location));
        }

        [Test]
        public void CommitsDrawingWhenPointerReleased()
        {
            Activate(BitmapLayer);

            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            commandDispatcher.Verify(d => d.Dispatch(new DrawCommand(BitmapLayer, drawable.Object)));
            Assert.That(Bitmap.GetDrawingPreview(), Is.Null);
        }

        [Test]
        public void DoesNotUpdateDrawingAfterPointerReleased()
        {
            Activate(BitmapLayer);

            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);
            tool.PointerMoved(PointerState.Empty);

            drawable.Verify(d => d.Update(It.IsAny<Point>()), Times.Once());
        }

        private class FakeDrawingTool : DrawingTool<IFakeDrawable>
        {
            private readonly IFakeDrawableFactory factory;

            public FakeDrawingTool(IFakeDrawableFactory factory)
            {
                this.factory = factory;
            }

            public override string Name => "Test";

            protected override IFakeDrawable CreateDrawable(Image image, PointerState p)
            {
                return factory.Create(image, p.Location);
            }

            protected override void UpdateDrawable(IFakeDrawable drawable, PointerState p)
            {
                drawable.Update(p.Location);
            }
        }

        public interface IFakeDrawableFactory
        {
            IFakeDrawable Create(Image image, Point p);
        }

        public interface IFakeDrawable : IDrawable
        {
            void Update(Point p);
        }
    }
}
