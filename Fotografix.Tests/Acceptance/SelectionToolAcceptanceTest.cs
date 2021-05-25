using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class SelectionToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task SelectsRectangle()
        {
            await OpenImageAsync("flowers.jpg");

            SelectTool("Selection");
            PressAndDragPointer(
                new Point(100, 100),
                new Point(200, 200)
            );

            await AssertImageAsync("flowers_selection_rectangle.png");
        }
    }
}
