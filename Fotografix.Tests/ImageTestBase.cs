using Fotografix.Composition;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Fotografix.Tests
{
    public abstract class ImageTestBase
    {
        protected async Task<StorageFile> GetFileAsync(string filename)
        {
            var imagesFolder = await Package.Current.InstalledLocation.GetFolderAsync("Images");
            return await imagesFolder.GetFileAsync(filename);
        }

        protected async Task<CanvasBitmap> LoadBitmapAsync(string filename)
        {
            var file = await GetFileAsync(filename);

            using (var stream = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
            }
        }

        protected async Task<Image> LoadImageAsync(string filename)
        {
            var bitmap = await LoadBitmapAsync(filename);
            return new Image(new BitmapLayer(bitmap));
        }

        protected async Task AssertImageAsync(string fileWithExpectedOutput, Image actualImage)
        {
            using (CanvasBitmap expected = await LoadBitmapAsync(fileWithExpectedOutput))
            using (CanvasBitmap actual = actualImage.Render())
            {
                AssertBytesAreEqual(expected.GetPixelBytes(), actual.GetPixelBytes(), 2);
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