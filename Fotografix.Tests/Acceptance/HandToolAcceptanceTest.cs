using Fotografix.Editor;
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
        public async Task ScrollsViewport()
        {
            await OpenImageAsync("flowers.jpg");
            SelectTool("Hand");

            AssertToolCursor(ToolCursor.OpenHand);

            Viewport.ScrollOffset = new PointF(10, 10);

            PressAndDragPointer(
                new Point(30, 30),
                new Point(32, 32)
            );

            AssertToolCursor(ToolCursor.ClosedHand);
            Assert.AreEqual(new PointF(8, 8), Viewport.ScrollOffset);
        }
    }
}
