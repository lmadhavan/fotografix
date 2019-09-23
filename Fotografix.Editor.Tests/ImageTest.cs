using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Editor.Tests
{
    [TestClass]
    public class ImageTest : EditorTestBase
    {
        private Image image;

        [TestInitialize]
        public async Task Initialize()
        {
            this.image = await LoadImageAsync("flowers.jpg");
        }

        [TestCleanup]
        public void Cleanup()
        {
            image.Dispose();
        }

        [TestMethod]
        public void ProvidesDimensions()
        {
            Assert.AreEqual(320, image.Width);
            Assert.AreEqual(480, image.Height);
        }
    }
}
