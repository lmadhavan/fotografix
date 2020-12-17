using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class CropToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task CropsImage()
        {
            await OpenImageAsync("flowers.jpg");
            
            var cropControls = SelectTool<ICropToolControls>("Crop");

            // drag top-left handle inwards
            DragAndReleasePointer(
                new Point(0, 0),
                new Point(100, 100)
            );

            // drag bottom-right handle inwards
            DragAndReleasePointer(
                new Point(320, 480),
                new Point(200, 200)
            );

            cropControls.Commit();

            await AssertImageAsync("flowers_crop.png");
        }
    }
}
