using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix.Editor.Tests
{
    [TestClass]
    public class ImageAdjustmentsTest : EditorTestBase
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
        public async Task BlackAndWhiteAdjustment()
        {
            using (BlackAndWhiteAdjustment adjustment = new BlackAndWhiteAdjustment())
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_bw.png", image);
            }
        }

        [TestMethod]
        public async Task BlackAndWhiteAdjustmentWithBlendMode()
        {
            using (BlackAndWhiteAdjustment adjustment = new BlackAndWhiteAdjustment(BlendMode.Multiply))
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_bw_multiply.png", image);
            }
        }

        [TestMethod]
        public async Task ShadowsHighlightsAdjustment()
        {
            using (ShadowsHighlightsAdjustment adjustment = new ShadowsHighlightsAdjustment()
            {
                Shadows = 0.25f,
                Highlights = -0.25f,
                Clarity = 0.5f
            })
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_sh.png", image);
            }
        }

        [TestMethod]
        public async Task HueSaturationAdjustment()
        {
            using (HueSaturationAdjustment adjustment = new HueSaturationAdjustment()
            {
                Hue = 0.5f,
                Saturation = 0.25f,
                Lightness = 0.25f
            })
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_hsl.png", image);
            }
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
