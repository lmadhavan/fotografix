using Fotografix.Win2D;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;

namespace Fotografix.Tests.Composition
{
    public abstract class CompositionTestBase
    {
        private readonly CanvasDevice canvasDevice;
        private readonly IBitmapFactory bitmapFactory;
        private readonly List<IBitmap> loadedBitmaps;

        protected CompositionTestBase()
        {
            this.canvasDevice = CanvasDevice.GetSharedDevice();
            this.bitmapFactory = new Win2DBitmapFactory(canvasDevice);
            this.loadedBitmaps = new List<IBitmap>();
        }

        [TestCleanup]
        public void CleanupBitmaps()
        {
            foreach (IBitmap bitmap in loadedBitmaps)
            {
                bitmap.Dispose();
            }
        }

        protected async Task<Image> LoadImageAsync(string filename)
        {
            var layer = await LoadLayerAsync(filename);

            Image image = new Image(layer.Bitmap.Size);
            image.Layers.Add(layer);
            return image;
        }

        protected async Task<BitmapLayer> LoadLayerAsync(string filename)
        {
            var file = await TestImages.GetFileAsync(filename);

            using (var stream = await file.OpenStreamForReadAsync())
            {
                var bitmap = await bitmapFactory.LoadBitmapAsync(stream);
                return new BitmapLayer(bitmap);
            }
        }

        protected async Task AssertImageAsync(string fileWithExpectedOutput, Image actualImage)
        {
            using (CanvasBitmap expected = await LoadBitmapAsync(fileWithExpectedOutput))
            using (CanvasBitmap actual = Render(actualImage))
            {
                AssertBytesAreEqual(expected.GetPixelBytes(), actual.GetPixelBytes(), 2);
            }
        }

        protected async Task RenderToTempFolderAsync(Image image, string filename)
        {
            using (var renderTarget = Render(image))
            {
                var folder = ApplicationData.Current.TemporaryFolder;
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.GenerateUniqueName);

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
                }

                FolderLauncherOptions options = new FolderLauncherOptions();
                options.ItemsToSelect.Add(file);
                await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await Launcher.LaunchFolderAsync(folder));
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

        private static CanvasRenderTarget Render(Image image)
        {
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), image.Size.Width, image.Size.Height, 96);

            using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
            using (Win2DCompositor compositor = new Win2DCompositor(image))
            {
                compositor.Draw(ds);
            }

            return renderTarget;
        }

        private async Task<CanvasBitmap> LoadBitmapAsync(string filename)
        {
            var file = await TestImages.GetFileAsync(filename);

            using (var stream = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(canvasDevice, stream);
            }
        }
    }
}