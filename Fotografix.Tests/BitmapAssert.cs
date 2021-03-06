﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    public static class BitmapAssert
    {
        public static async Task AreEquivalentAsync(string fileWithExpectedOutput, Bitmap actual)
        {
            Bitmap expected = await TestImages.LoadBitmapAsync(fileWithExpectedOutput);
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
