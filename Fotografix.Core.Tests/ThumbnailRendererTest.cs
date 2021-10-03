using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Foundation;

namespace Fotografix
{
    [TestClass]
    public class ThumbnailRendererTest
    {
        [TestMethod]
        public void ComputesThumbnailSize()
        {
            ThumbnailRenderer renderer = new ThumbnailRenderer(thumbnailSize: 100);

            Assert.AreEqual(new Size(100, 50), renderer.GetThumbnailSize(new Size(1000, 500)));
            Assert.AreEqual(new Size(50, 100), renderer.GetThumbnailSize(new Size(500, 1000)));
        }
    }
}
