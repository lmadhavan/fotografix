using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class HandToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task PansViewport()
        {
            await OpenImageAsync("flowers.jpg");
            await Workspace.ResetZoomCommand.ExecuteAsync();
            ResizeViewport(new Size(200, 200));
            SelectTool("Hand");

            AssertToolCursor(ToolCursor.OpenHand);

            PressAndDragPointer(
                new Point(150, 150),
                new Point(50, 50)
            );

            AssertToolCursor(ToolCursor.ClosedHand);
            await AssertImageAsync("flowers_viewport_pan.png");
        }
    }
}
