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
        private BrushStroke capturedBrushStroke;

        [SetUp]
        public void SetUp()
        {
            this.tool = new BrushTool()
            {
                Size = BrushSize,
                Color = BrushColor
            };

            this.drawingSurface = new Mock<IDrawingSurface>();
            var captureMatch = new CaptureMatch<BrushStroke>(d => this.capturedBrushStroke = d);
            drawingSurface.Setup(ds => ds.BeginDrawing(Capture.With(captureMatch)));
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

            Assert.Pass();
        }

        [Test]
        public void BeginsDrawingWhenPointerPressed()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);

            drawingSurface.Verify(ds => ds.BeginDrawing(It.IsAny<BrushStroke>()));

            Assert.That(capturedBrushStroke.Size, Is.EqualTo(BrushSize));
            Assert.That(capturedBrushStroke.Color, Is.EqualTo(BrushColor));
            Assert.That(capturedBrushStroke.Points, Is.EqualTo(new PointF[] { Start.Location }));
        }

        [Test]
        public void UpdatesDrawingWhenPointerMoved()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);

            Assert.That(capturedBrushStroke.Points, Is.EqualTo(new PointF[] { Start.Location, End.Location }));
        }

        [Test]
        public void EndsDrawingWhenPointerReleased()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            drawingSurface.Verify(ds => ds.EndDrawing(capturedBrushStroke));
        }

        [Test]
        public void DoesNotUpdateDrawingAfterPointerReleased()
        {
            tool.DrawingSurfaceActivated(drawingSurface.Object);
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);
            tool.PointerMoved(PointerState.Empty);

            Assert.That(capturedBrushStroke.Points, Has.Count.EqualTo(2));
        }
    }
}
