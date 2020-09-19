using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    public static class AssertImage
    {
        public static async Task IsEquivalentAsync(string fileWithExpectedOutput, Bitmap actual)
        {
            var file = await TestImages.GetFileAsync(fileWithExpectedOutput);
            Bitmap expected = await BitmapCodec.LoadBitmapAsync(file);
            AssertBytesAreEqual(expected.Pixels, actual.Pixels, 3, fileWithExpectedOutput);
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
