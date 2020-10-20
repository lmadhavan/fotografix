using Fotografix.Editor.Tools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [Ignore]
    [TestClass]
    public class CropToolAcceptanceTest : ToolAcceptanceTestBase
    {
        [TestMethod]
        public async Task CropsImage()
        {
            await OpenImageAsync("flowers.jpg");
            
            var cropControls = SelectTool<ICropToolControls>("Crop");

            // move top-left handle inwards
            DragAndReleasePointer(
                new Point(0, 0),
                new Point(100, 100)
            );

            // move bottom-right handle inwards
            DragAndReleasePointer(
                new Point(320, 480),
                new Point(200, 200)
            );

            await SaveToTempFolderAsync("flowers_crop_preview.png");

            cropControls.Apply();

            await SaveToTempFolderAsync("flowers_crop.png");
        }
    }
}
