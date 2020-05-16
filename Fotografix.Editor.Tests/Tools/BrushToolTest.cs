using Fotografix.Adjustments;
using Fotografix.Editor.Tools;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tests.Tools
{
    [TestFixture]
    public class BrushToolTest
    {
        private static readonly PointF Start = new PointF(10, 10);
        private static readonly PointF End = new PointF(20, 20);

        private BrushTool tool;
        private BrushStrokeEventArgs brushStrokeCompletedEvent;

        [SetUp]
        public void SetUp()
        {
            this.tool = new BrushTool();
            tool.BrushStrokeCompleted += (s, e) => this.brushStrokeCompletedEvent = e;
        }

        [Test]
        public void EnabledOnBitmapLayer()
        {
            ActivateBitmapLayer();

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Crosshair));
        }

        [Test]
        public void TranslatesPointerDragEventIntoBrushStroke()
        {
            tool.Size = 5;
            tool.Color = Color.White;

            ActivateBitmapLayer();
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            Assert.That(brushStrokeCompletedEvent, Is.Not.Null);

            BrushStroke brushStroke = brushStrokeCompletedEvent.BrushStroke;
            Assert.That(brushStroke.Size, Is.EqualTo(5));
            Assert.That(brushStroke.Color, Is.EqualTo(Color.White));
            Assert.That(brushStroke.Points, Is.EqualTo(new PointF[] { Start, End }));
        }

        [Test]
        public void DisabledOnNonBitmapLayer()
        {
            ActivateNonBitmapLayer();

            Assert.That(tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }
        
        [Test]
        public void IgnoresPointerMovementWhenDisabled()
        {
            ActivateNonBitmapLayer();
            tool.PointerPressed(Start);
            tool.PointerMoved(End);
            tool.PointerReleased(End);

            Assert.That(brushStrokeCompletedEvent, Is.Null);
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
