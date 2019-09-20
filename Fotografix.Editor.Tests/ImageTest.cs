using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
                AssertBytesAreEqual(expected.GetPixelBytes(), actual.GetPixelBytes(), 1);
            }
        }

        private void AssertBytesAreEqual(byte[] expected, byte[] actual, byte tolerance)
        {
            if (expected.Length != actual.Length)
            {
                Assert.Fail("Content length differs: expected = {0}, actual = {1}", expected.Length, actual.Length);
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (Math.Abs(expected[i] - actual[i]) > tolerance)
                {
                    Assert.Fail($"Content differs at index {i}: expected = {expected[i]} ± {tolerance}, actual = {actual[i]}");
                }
            }
        }

    }
}
