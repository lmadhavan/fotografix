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
        public async Task DrawsBrushStroke()
        {
            await OpenImageAsync("flowers.jpg");
            ConfigureBrushTool(size: 15, Color.White);

            AssertToolCursor(ToolCursor.Crosshair);

            PressAndDragPointer(new Point[] {
                new Point(100, 100),
                new Point(250, 150),
                new Point(250, 350)
            });

            await AssertImageAsync("flowers_brush_preview.png");

            ContinueDraggingAndReleasePointer(new Point[]
            {
                new Point(75, 200),
                new Point(200, 50)
            });

            await AssertImageAsync("flowers_brush.png");

            Undo();

            await AssertImageAsync("flowers.jpg");
        }

        private void ConfigureBrushTool(int size, Color color)
        {
            IBrushToolSettings settings = SelectTool<IBrushToolSettings>("Brush");
            settings.Size = size;
            settings.Color = color;
        }
    }
}
