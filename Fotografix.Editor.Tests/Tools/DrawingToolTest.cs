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
        private Mock<IDrawingSurface> drawingSurface;
        private Mock<IFakeDrawableFactory> drawableFactory;
        private Mock<IFakeDrawable> drawable;

        [SetUp]
        public void SetUp()
        {
            this.drawingSurface = new Mock<IDrawingSurface>();
            this.drawableFactory = new Mock<IFakeDrawableFactory>();
            this.drawable = new Mock<IFakeDrawable>();
            drawableFactory.Setup(f => f.Create(It.IsAny<Point>())).Returns(drawable.Object);

            this.tool = new FakeDrawingTool(drawableFactory.Object);
        }

        [Test]
        public void DisabledWhenNoDrawingSurfaceIsActive()
        {
            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void EnabledWhenDrawingSurfaceIsActive()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Crosshair));
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
        public void BeginsDrawingWhenPointerPressed()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);

            drawableFactory.Verify(f => f.Create(Start.Location));
            drawingSurface.Verify(ds => ds.BeginDrawing(drawable.Object));
        }

        [Test]
        public void UpdatesDrawingWhenPointerMoved()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);

            drawable.Verify(d => d.Update(End.Location));
        }

        [Test]
        public void EndsDrawingWhenPointerReleased()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            drawingSurface.Verify(ds => ds.EndDrawing(drawable.Object));
        }

        [Test]
        public void DoesNotUpdateDrawingAfterPointerReleased()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
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

            protected override object Settings => this;

            protected override IFakeDrawable CreateDrawable(PointerState p)
            {
                return factory.Create(p.Location);
            }

            protected override void UpdateDrawable(IFakeDrawable drawable, PointerState p)
            {
                drawable.Update(p.Location);
            }
        }

        public interface IFakeDrawableFactory
        {
            IFakeDrawable Create(Point p);
        }

        public interface IFakeDrawable : IDrawable
        {
            void Update(Point p);
        }
    }
}
