using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;

namespace Fotografix
{
    internal static class BitmapAssert
    {
        internal const float DefaultTolerance = 1.75f;

        internal static async Task VerifyAsync(StorageFile actual, string fileWithExpectedBitmap, float tolerance = DefaultTolerance)
        {
            using (var stream = await actual.OpenReadAsync())
            {
                await VerifyAsync(stream, fileWithExpectedBitmap, tolerance);
            }
        }

        internal static async Task VerifyAsync(IRandomAccessStream actual, string fileWithExpectedBitmap, float tolerance = DefaultTolerance)
        {
            using (var actualBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), actual))
            {
                await VerifyAsync(actualBitmap, fileWithExpectedBitmap, tolerance);
            }
        }

        internal static async Task VerifyAsync(CanvasBitmap actual, string fileWithExpectedBitmap, float tolerance = DefaultTolerance)
        {
            try
            {
                var file = await TestData.GetFileAsync(fileWithExpectedBitmap);

                using (var stream = await file.OpenReadAsync())
                using (var expected = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream))
                {
                    VerifyBytes(expected.GetPixelBytes(), actual.GetPixelBytes(), tolerance, fileWithExpectedBitmap);
                }
            }
            catch
            {
                await SaveAsync(actual, fileWithExpectedBitmap);
                throw;
            }
        }

        private static async Task SaveAsync(CanvasBitmap bitmap, string filename)
        {
            var tempFolder = ApplicationData.Current.TemporaryFolder;
            var file = await tempFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                await bitmap.SaveAsync(stream, CanvasBitmapFileFormat.Jpeg);
            }

            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await Launcher.LaunchFolderAsync(tempFolder));
        }

        private static void VerifyBytes(byte[] expected, byte[] actual, float tolerance, string filename)
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

            if (avgDelta > tolerance)
            {
                Assert.Fail($"Average delta exceeds tolerance ({filename}): {avgDelta} > {tolerance}");
            }

            Debug.WriteLine($"Average delta for {filename} = {avgDelta}");
        }
    }
}
