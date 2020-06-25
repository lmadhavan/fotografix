using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    [TestClass]
    public class BrushToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task PaintsBrushStrokeFromPoints()
        {
            await OpenImageAsync("flowers.jpg");
            ConfigureBrushTool(size: 15, Color.White);

            AssertToolCursor(ToolCursor.Crosshair);

            PressAndDragPointer(new PointF[] {
                new PointF(100, 100),
                new PointF(250, 150),
                new PointF(250, 350)
            });

            await AssertImageAsync("flowers_brush_partial.png");

            ContinueDraggingAndReleasePointer(new PointF[]
            {
                new PointF(75, 200),
                new PointF(200, 50)
            });

            await AssertImageAsync("flowers_brush.png");
            AssertCanUndo();

            Undo();

            await AssertImageAsync("flowers.jpg");
        }

        private void ConfigureBrushTool(float size, Color color)
        {
            IBrushToolSettings settings = SelectTool<IBrushToolSettings>("Brush");
            settings.Size = size;
            settings.Color = color;
        }
    }
}
