using Fotografix.Adjustments;
using Fotografix.Drawing;
using Fotografix.Editor.Drawing;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class DrawingToolTest
    {
        private static readonly PointerState Start = new PointerState(new Point(10, 10));
        private static readonly PointerState End = new PointerState(new Point(20, 20));

        private FakeDrawingTool tool;
        
        private Image image;
        private Bitmap bitmap;
        private BitmapLayer bitmapLayer;
        private Layer nonBitmapLayer;

        private Mock<IFakeDrawableFactory> drawableFactory;
        private Mock<IFakeDrawable> drawable;
        private Mock<ICommandDispatcher> commandDispatcher;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(new Size(10, 10));
            this.bitmap = new Bitmap(new Size(10, 10));
            this.bitmapLayer = new BitmapLayer(bitmap);
            this.nonBitmapLayer = new AdjustmentLayer(new BlackAndWhiteAdjustment());

            this.drawableFactory = new Mock<IFakeDrawableFactory>();
            this.drawable = new Mock<IFakeDrawable>();
            drawableFactory.Setup(f => f.Create(It.IsAny<Image>(), It.IsAny<Point>())).Returns(drawable.Object);

            this.commandDispatcher = new Mock<ICommandDispatcher>();
            image.SetCommandDispatcher(commandDispatcher.Object);

            this.tool = new FakeDrawingTool(drawableFactory.Object);
        }

        [Test]
        public void DisabledWhenNotActivated()
        {
            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void DisabledWhenActiveLayerIsNotBitmapLayer()
        {
            image.SetActiveLayer(nonBitmapLayer);
            tool.Activated(image);

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void EnabledWhenActiveLayerIsBitmapLayer()
        {
            image.SetActiveLayer(bitmapLayer);
            tool.Activated(image);

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Crosshair));
        }

        [Test]
        public void DisabledWhenActiveLayerChangesToNonBitmapLayerAfterActivation()
        {
            image.SetActiveLayer(bitmapLayer);
            tool.Activated(image);
            image.SetActiveLayer(nonBitmapLayer);

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void IgnoresPointerEventsWhenDisabled()
        {
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            drawableFactory.VerifyNoOtherCalls();
        }

        [Test]
        public void BeginsDrawingPreviewWhenPointerPressed()
        {
            image.SetActiveLayer(bitmapLayer);
            tool.Activated(image);

            tool.PointerPressed(Start);

            drawableFactory.Verify(f => f.Create(image, Start.Location));
            Assert.That(bitmapLayer.GetDrawingPreview(), Is.EqualTo(drawable.Object));
        }

        [Test]
        public void UpdatesDrawingWhenPointerMoved()
        {
            image.SetActiveLayer(bitmapLayer);
            tool.Activated(image);

            tool.PointerPressed(Start);
            tool.PointerMoved(End);

            drawable.Verify(d => d.Update(End.Location));
        }

        [Test]
        public void CommitsDrawingWhenPointerReleased()
        {
            image.SetActiveLayer(bitmapLayer);
            tool.Activated(image);

            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            commandDispatcher.Verify(d => d.Dispatch(new DrawCommand(bitmapLayer, drawable.Object)));
            Assert.That(bitmapLayer.GetDrawingPreview(), Is.Null);
        }

        [Test]
        public void DoesNotUpdateDrawingAfterPointerReleased()
        {
            image.SetActiveLayer(bitmapLayer);
            tool.Activated(image);

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
