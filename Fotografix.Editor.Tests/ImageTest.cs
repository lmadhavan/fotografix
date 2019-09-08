using Microsoft.Graphics.Canvas;
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

        [TestMethod]
        public async Task AppliesBlackAndWhiteAdjustment()
        {
            image.ApplyBlackAndWhiteAdjustment();
            await AssertImageAsync("flowers_bw.png", image);
        }

        [TestMethod]
        public async Task AppliesBlackAndWhiteAdjustmentWithBlendMode()
        {
            image.ApplyBlackAndWhiteAdjustment(BlendMode.Multiply);
            await AssertImageAsync("flowers_bw_multiply.png", image);
        }

        private async Task AssertImageAsync(string fileWithExpectedOutput, Image actualImage)
        {
            using (CanvasBitmap expected = await LoadBitmapAsync(fileWithExpectedOutput))
            using (CanvasBitmap actual = actualImage.Render())
            {
                AssertBytesAreEqual(expected.GetPixelBytes(), actual.GetPixelBytes());
            }
        }
        private void AssertBytesAreEqual(byte[] expected, byte[] actual)
        {
            if (expected.Length != actual.Length)
            {
                Assert.Fail("Content length differs: expected = {0}, actual = {1}", expected.Length, actual.Length);
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != actual[i])
                {
                    Assert.Fail("Content differs at index {0}: expected = {1}, actual = {2}", i, expected[i], actual[i]);
                }
            }
        }
    }
}
