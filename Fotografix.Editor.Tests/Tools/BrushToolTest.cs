using Fotografix.Adjustments;
using Fotografix.Testing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class BrushToolTest
    {
        private const int BrushSize = 5;
        private static readonly Color BrushColor = Color.White;

        private static readonly PointF Start = new PointF(10, 10);
        private static readonly PointF End = new PointF(20, 20);

        private BrushTool tool;
        private BrushStrokeEventArgs brushStrokeStartedEvent;
        private BrushStrokeEventArgs brushStrokeCompletedEvent;

        [SetUp]
        public void SetUp()
        {
            this.tool = new BrushTool()
            {
                Size = BrushSize,
                Color = BrushColor
            };
            tool.BrushStrokeStarted += (s, e) => this.brushStrokeStartedEvent = e;
            tool.BrushStrokeCompleted += (s, e) => this.brushStrokeCompletedEvent = e;
        }

        [Test]
        public void EnabledOnBitmapLayer()
        {
            ActivateBitmapLayer();

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Crosshair));
        }

        [Test]
        public void DisabledOnNonBitmapLayer()
        {
            ActivateNonBitmapLayer();

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void IgnoresPointerEventsWhenDisabled()
        {
            ActivateNonBitmapLayer();
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            Assert.That(brushStrokeStartedEvent, Is.Null);
            Assert.That(brushStrokeCompletedEvent, Is.Null);
        }

        [Test]
        public void PointerPressStartsBrushStroke()
        {
            ActivateBitmapLayer();
            tool.PointerPressed(Start);

            Assert.That(brushStrokeStartedEvent, Is.Not.Null);
            Assert.That(brushStrokeStartedEvent.BrushStroke.Size, Is.EqualTo(BrushSize));
            Assert.That(brushStrokeStartedEvent.BrushStroke.Color, Is.EqualTo(BrushColor));
            Assert.That(brushStrokeStartedEvent.BrushStroke.Points, Is.EqualTo(new PointF[] { Start }));
        }

        [Test]
        public void PointerDragAndReleaseCompletesBrushStroke()
        {
            ActivateBitmapLayer();
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            Assert.That(brushStrokeCompletedEvent, Is.Not.Null);
            Assert.That(brushStrokeCompletedEvent.BrushStroke, Is.SameAs(brushStrokeStartedEvent.BrushStroke));
            Assert.That(brushStrokeCompletedEvent.BrushStroke.Points, Is.EqualTo(new PointF[] { Start, End }));
        }

        [Test]
        public void StopsUpdatingBrushStrokeAfterPointerIsReleased()
        {
            ActivateBitmapLayer();
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);
            tool.PointerMoved(PointF.Empty);

            Assert.That(brushStrokeCompletedEvent.BrushStroke.Points, Has.Count.EqualTo(2));
        }

        private void ActivateBitmapLayer()
        {
            tool.LayerActivated(new BitmapLayer(new FakeBitmap()));
        }

        private void ActivateNonBitmapLayer()
        {
            tool.LayerActivated(new AdjustmentLayer(new BlackAndWhiteAdjustment()));
        }
    }
}
