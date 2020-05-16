using Fotografix.Adjustments;
using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    [TestClass]
    public class BrushToolAcceptanceTest : AcceptanceTestBase
    {
        private static readonly PointF[] Points = new PointF[] {
            new PointF(100, 100),
            new PointF(250, 150),
            new PointF(250, 350),
            new PointF(75, 200),
            new PointF(200, 50)
        };

        [TestMethod]
        public async Task PaintsBrushStrokeFromPoints()
        {
            await OpenImageAsync("flowers.jpg");
            ConfigureBrushTool(size: 15, Color.White);

            AssertToolCursor(ToolCursor.Crosshair);

            DragPointer(Points);

            await AssertImageAsync("flowers_brush.png");
        }

        [TestMethod]
        public async Task DisabledOnNonBitmapLayer()
        {
            await OpenImageAsync("flowers.jpg");
            ConfigureBrushTool(size: 15, Color.White);
            AddAdjustmentLayer<BlackAndWhiteAdjustment>();

            AssertToolCursor(ToolCursor.Disabled);
        }

        private void ConfigureBrushTool(float size, Color color)
        {
            IBrushToolSettings settings = (IBrushToolSettings)Editor.ToolSettings;
            settings.Size = size;
            settings.Color = color;
        }

        private void AssertToolCursor(ToolCursor expected)
        {
            Assert.AreEqual(expected, Editor.ToolCursor);
        }

        private void DragPointer(PointF[] points)
        {
            Editor.PointerPressed(points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                Editor.PointerMoved(points[i]);
            }

            Editor.PointerReleased(points[points.Length - 1]);
        }
    }
}
