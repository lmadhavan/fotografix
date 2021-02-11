using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class ViewportAcceptanceTest : AcceptanceTestBase
    {
        [TestMethod]
        public async Task CentersImageInViewport()
        {
            await OpenImageAsync("flowers.jpg");

            ResizeViewport(new Size(600, 800));

            await AssertImageAsync("flowers_viewport_center.png");
        }
    }
}
