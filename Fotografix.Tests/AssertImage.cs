using Fotografix.UI;
using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    public static class AssertImage
    {
        public static async Task IsEquivalentAsync(string fileWithExpectedOutput, Image actualImage)
        {
            var file = await TestImages.GetFileAsync(fileWithExpectedOutput);
            Bitmap expected = await BitmapLoader.LoadBitmapAsync(file);

            using (Win2DCompositor compositor = new Win2DCompositor(actualImage))
            using (CanvasBitmap actual = TestRenderer.Render(compositor))
            {
                AssertBytesAreEqual(expected.Pixels, actual.GetPixelBytes(), 3);
            }
        }

        private static void AssertBytesAreEqual(byte[] expected, byte[] actual, byte tolerance)
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
