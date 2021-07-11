using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class ClipboardAcceptanceTest : AcceptanceTestBase
    {
        [TestMethod]
        public async Task PastesBitmapFromClipboard()
        {
            await OpenImageAsync("flowers.jpg");

            var bitmap = await TestImages.LoadBitmapAsync("stars_small.jpg");
            Clipboard.SetBitmap(bitmap);

            await Workspace.PasteCommand.ExecuteAsync();

            await AssertImageAsync("flowers_stars_small_center.png");
        }
    }
}
