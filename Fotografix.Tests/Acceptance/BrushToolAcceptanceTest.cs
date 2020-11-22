using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class BrushToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task DrawsBrushStroke()
        {
            await OpenImageAsync("flowers.jpg");
            ConfigureBrushTool(size: 15, Color.White);

            AssertToolCursor(ToolCursor.Crosshair);

            PressAndDragPointer(
                new Point(100, 100),
                new Point(250, 150),
                new Point(250, 350)
            );

            await AssertImageAsync("flowers_brush_preview.png");

            ContinueDraggingAndReleasePointer(
                new Point(75, 200),
                new Point(200, 50)
            );

            await AssertImageAsync("flowers_brush.png");
        }

        private void ConfigureBrushTool(int size, Color color)
        {
            var brushControls = SelectTool<IBrushToolControls>("Brush");
            brushControls.Size = size;
            brushControls.Color = color;
        }
    }
}
