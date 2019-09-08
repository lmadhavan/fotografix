using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Fotografix.Editor.Tests
{
    [TestClass]
    public class ImageTest
    {
        private StorageFolder imagesFolder;
        private Image image;

        [TestInitialize]
        public async Task Initialize()
        {
            this.imagesFolder = await Package.Current.InstalledLocation.GetFolderAsync("Images");
            var flowers = await imagesFolder.GetFileAsync("flowers.jpg");

            this.image = await Image.LoadAsync(flowers);
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

            using (CanvasBitmap expected = await LoadImageAsync("flowers_bw.png"))
            using (CanvasBitmap actual = image.Render())
            {
                AssertBytesAreEqual(expected.GetPixelBytes(), actual.GetPixelBytes());
            }
        }

        private async Task<CanvasBitmap> LoadImageAsync(string filename)
        {
            var file = await imagesFolder.GetFileAsync(filename);

            using (var stream = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
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
