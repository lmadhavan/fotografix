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
        private static readonly IBitmapFactory BitmapFactory = new Win2DBitmapFactory();

        public static async Task IsEquivalentAsync(string fileWithExpectedOutput, IWin2DDrawable actual)
        {
            var file = await TestImages.GetFileAsync(fileWithExpectedOutput);

            using (Bitmap expectedBitmap = await BitmapCodec.LoadBitmapAsync(file, BitmapFactory))
            using (CanvasBitmap actualBitmap = TestRenderer.Render(actual))
            {
                AssertBytesAreEqual(expectedBitmap.GetPixelBytes(), actualBitmap.GetPixelBytes(), 3, fileWithExpectedOutput);
            }
        }

        private static void AssertBytesAreEqual(byte[] expected, byte[] actual, byte tolerance, string fileWithExpectedOutput)
        {
            if (expected.Length != actual.Length)
            {
                Assert.Fail($"Content length differs: expected = {expected.Length} ({fileWithExpectedOutput}), actual = {actual.Length}");
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (Math.Abs(expected[i] - actual[i]) > tolerance)
                {
                    Assert.Fail($"Content differs at index {i}: expected = {expected[i]} ± {tolerance} ({fileWithExpectedOutput}), actual = {actual[i]}");
                }
            }
        }
    }
}
