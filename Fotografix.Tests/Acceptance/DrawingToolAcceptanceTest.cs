using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class DrawingToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task DrawsBrushStroke()
        {
            await OpenImageAsync("flowers.jpg");

            var brushControls = SelectTool<IBrushToolControls>("Brush");
            brushControls.Size = 15;
            Workspace.Colors.ForegroundColor = Color.White;

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

        [TestMethod]
        public async Task DrawsLinearGradient()
        {
            await OpenImageAsync("flowers.jpg");

            SelectTool("Gradient");
            Workspace.Colors.ForegroundColor = Color.Red;
            Workspace.Colors.BackgroundColor = Color.Green;

            AssertToolCursor(ToolCursor.Crosshair);

            DragAndReleasePointer(
                new Point(100, 100),
                new Point(300, 300)
            );

            await AssertImageAsync("gradient.png");
        }

        [TestMethod]
        public async Task ClipsDrawingToSelection()
        {
            await OpenImageAsync("flowers.jpg");

            SelectTool("Selection");
            DragAndReleasePointer(
                new Point(100, 150),
                new Point(200, 350)
            );

            SelectTool("Gradient");
            Workspace.Colors.ForegroundColor = Color.Red;
            Workspace.Colors.BackgroundColor = Color.Green;

            DragAndReleasePointer(
                new Point(100, 100),
                new Point(300, 300)
            );

            await AssertImageAsync("flowers_selection_rectangle_gradient.png");
        }
    }
}
