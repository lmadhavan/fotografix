using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class BrushToolTest
    {
        private const int BrushSize = 5;
        private static readonly Color BrushColor = Color.White;

        private static readonly PointerState Start = new PointerState(new Point(10, 10));
        private static readonly PointerState End = new PointerState(new Point(20, 20));

        private BrushTool tool;
        private Mock<IDrawingSurface> drawingSurface;
        private Mock<IBrushStrokeFactory> brushStrokeFactory;
        private Mock<IBrushStroke> brushStroke;

        [SetUp]
        public void SetUp()
        {
            this.drawingSurface = new Mock<IDrawingSurface>();
            this.brushStrokeFactory = new Mock<IBrushStrokeFactory>();
            this.brushStroke = new Mock<IBrushStroke>();

            brushStrokeFactory.Setup(f => f.CreateBrushStroke(It.IsAny<Point>(), It.IsAny<int>(), It.IsAny<Color>())).Returns(brushStroke.Object);

            this.tool = new BrushTool(brushStrokeFactory.Object)
            {
                Size = BrushSize,
                Color = BrushColor
            };
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

            brushStrokeFactory.VerifyNoOtherCalls();
        }

        [Test]
        public void BeginsDrawingWhenPointerPressed()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);

            brushStrokeFactory.Verify(f => f.CreateBrushStroke(Start.Location, BrushSize, BrushColor));
            drawingSurface.Verify(ds => ds.BeginDrawing(brushStroke.Object));
        }

        [Test]
        public void UpdatesDrawingWhenPointerMoved()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);

            brushStroke.Verify(bs => bs.AddPoint(End.Location));
        }

        [Test]
        public void EndsDrawingWhenPointerReleased()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            drawingSurface.Verify(ds => ds.EndDrawing(brushStroke.Object));
        }

        [Test]
        public void DoesNotUpdateDrawingAfterPointerReleased()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);
            tool.PointerMoved(PointerState.Empty);

            brushStroke.Verify(bs => bs.AddPoint(It.IsAny<Point>()), Times.Once());
        }
    }
}
