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

            var brushControls = SelectTool<IBrushToolControls>("Brush");
            brushControls.Size = 15;
            brushControls.Color = Color.White;

            AssertToolCursor(ToolCursor.Crosshair);

            DragAndReleasePointer(
                new Point(100, 100),
                new Point(250, 150),
                new Point(250, 350),
                new Point(75, 200),
                new Point(200, 50)
            );

            await AssertImageAsync("flowers_brush.png");
        }
    }
}
