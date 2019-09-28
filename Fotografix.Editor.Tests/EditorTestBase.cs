using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Fotografix.Editor.Tests
{
    public abstract class EditorTestBase
    {
        private StorageFolder imagesFolder;

        [TestInitialize]
        public async Task InitializeImagesFolder()
        {
            this.imagesFolder = await Package.Current.InstalledLocation.GetFolderAsync("Images");
        }

        protected async Task<Image> LoadImageAsync(string filename)
        {
            var file = await imagesFolder.GetFileAsync(filename);
            return await Image.LoadAsync(file);
        }

        protected async Task AssertImageAsync(string fileWithExpectedOutput, Image actualImage)
        {
            using (CanvasBitmap expected = await LoadBitmapAsync(fileWithExpectedOutput))
            using (CanvasBitmap actual = actualImage.Render())
            {
                AssertBytesAreEqual(expected.GetPixelBytes(), actual.GetPixelBytes(), 1);
            }
        }

        private async Task<CanvasBitmap> LoadBitmapAsync(string filename)
        {
            var file = await imagesFolder.GetFileAsync(filename);

            using (var stream = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
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