using Fotografix.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class ResizeImageAcceptanceTest : AcceptanceTestBase
    {
        [TestMethod]
        public async Task ResizingImageScalesBitmapLayers()
        {
            await OpenImageAsync("flowers.jpg");

            await Editor.ResizeImageCommand.ExecuteAsync();

            await AssertImageAsync("flowers_scale50.png");
        }

        protected override bool HandleResizeImageDialog(ResizeImageParameters parameters)
        {
            parameters.LockAspectRatio = true;
            parameters.Width /= 2;
            return true;
        }
    }
}
