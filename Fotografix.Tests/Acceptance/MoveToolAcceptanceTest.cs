using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class MoveToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task MovesLayer()
        {
            await OpenImageAsync("flowers.jpg");
            await ImportLayerAsync("stars_small.jpg");
            
            SelectTool("Move");
            PressAndDragPointer(
                new Point(100, 100),
                new Point(150, 150)
            );

            await AssertImageAsync("flowers_stars_small_offset.png");
        }
    }
}
