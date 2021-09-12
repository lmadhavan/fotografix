using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;

namespace Fotografix
{
    [TestClass]
    public class PhotoAdjustmentTest
    {
        private const float Tolerance = 1.75f;

        private CanvasBitmap bitmap;
        private PhotoAdjustment adjustment;

        [TestInitialize]
        public async Task Initialize()
        {
            var folder = await TestData.GetFolderAsync("Photos");
            var file = await folder.GetFileAsync("Barn.jpg");

            using (var stream = await file.OpenReadAsync())
            {
                this.bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
            }

            this.adjustment = new PhotoAdjustment();
        }

        [TestCleanup]
        public void Cleanup()
        {
            adjustment.Dispose();
            bitmap.Dispose();
        }

        [TestMethod]
        public async Task Exposure()
        {
            adjustment.Exposure = 0.5f;
            await VerifyOutputAsync("Barn_exposure.jpg");
        }

        [TestMethod]
        public async Task Highlights()
        {
            adjustment.Highlights = 0.5f;
            await VerifyOutputAsync("Barn_highlights.jpg");
        }

        [TestMethod]
        public async Task Shadows()
        {
            adjustment.Shadows = 0.5f;
            await VerifyOutputAsync("Barn_shadows.jpg");
        }

        [TestMethod]
        public async Task Temperature()
        {
            adjustment.Temperature = 0.5f;
            await VerifyOutputAsync("Barn_temperature.jpg");
        }

        [TestMethod]
        public async Task Tint()
        {
            adjustment.Tint = 0.5f;
            await VerifyOutputAsync("Barn_tint.jpg");
        }

        private async Task VerifyOutputAsync(string filename)
        {
            try
            {
                var file = await TestData.GetFileAsync(filename);

                using (var stream = await file.OpenReadAsync())
                using (var expected = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream))
                using (var actual = Render())
                {
                    VerifyBytes(expected.GetPixelBytes(), actual.GetPixelBytes(), filename);
                }
            }
            catch
            {
                await CaptureOutputAsync(filename);
                throw;
            }
        }

        private async Task CaptureOutputAsync(string filename)
        {
            var tempFolder = ApplicationData.Current.TemporaryFolder;
            var file = await tempFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (var output = Render())
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await output.SaveAsync(stream, CanvasBitmapFileFormat.Jpeg);
            }

            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await Launcher.LaunchFolderAsync(tempFolder));
        }

        private CanvasBitmap Render()
        {
            var rt = new CanvasRenderTarget(bitmap, bitmap.Size);

            using (var ds = rt.CreateDrawingSession())
            {
                adjustment.Render(ds, bitmap);
            }

            return rt;
        }

        private static void VerifyBytes(byte[] expected, byte[] actual, string filename)
        {
            if (expected.Length != actual.Length)
            {
                Assert.Fail($"Content length differs: expected = {expected.Length} ({filename}), actual = {actual.Length}");
            }

            int totalDelta = 0;

            for (int i = 0; i < expected.Length; i++)
            {
                int delta = Math.Abs(expected[i] - actual[i]);
                totalDelta += delta;
            }

            float avgDelta = (float)totalDelta / expected.Length;

            if (avgDelta > Tolerance)
            {
                Assert.Fail($"Average delta exceeds tolerance ({filename}): {avgDelta} > {Tolerance}");
            }

            Debug.WriteLine($"Average delta for {filename} = {avgDelta}");
        }
    }
}
