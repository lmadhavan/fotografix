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
        private static IBitmapFactory bitmapFactory = new Win2DBitmapFactory();

        public static async Task IsEquivalentAsync(string fileWithExpectedOutput, IWin2DDrawable actual)
        {
            var file = await TestImages.GetFileAsync(fileWithExpectedOutput);

            using (Bitmap expectedBitmap = await BitmapLoader.LoadBitmapAsync(file, bitmapFactory))
            using (CanvasBitmap actualBitmap = TestRenderer.Render(actual))
            {
                AssertBytesAreEqual(expectedBitmap.GetPixelBytes(), actualBitmap.GetPixelBytes(), 3);
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
