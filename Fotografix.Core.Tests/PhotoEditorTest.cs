using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Fotografix
{
    [TestClass]
    public class PhotoEditorTest
    {
        private StorageFileReference sidecar;
        private Photo photo;

        [TestInitialize]
        public async Task Initialize()
        {
            var content = await TestData.GetFileAsync("Photos\\Barn.jpg");
            this.sidecar = new StorageFileReference(ApplicationData.Current.TemporaryFolder, "Barn.dat");

            this.photo = new Photo(content, sidecar);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await sidecar.DeleteAsync();
        }

        [TestMethod]
        public async Task AppliesAdjustmentToPhoto()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                await VerifyOutputAsync(editor, "Barn_exposure.jpg");
            }
        }

        [TestMethod]
        public async Task ResetsAdjustment()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                editor.ResetAdjustment();
                await VerifyOutputAsync(editor, "Photos\\Barn.jpg");
            }
        }

        [TestMethod]
        public async Task PreservesRenderScaleWhenAdjustmentIsReset()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.RenderScale = 0.5f;
                editor.ResetAdjustment();

                Assert.AreEqual(0.5f, editor.RenderScale);
            }
        }

        [TestMethod]
        public async Task ComputesRenderScaleBasedOnSize()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.SetRenderSize(new Size(300, 150));

                // original photo is 900x600, so 300x150 should result in 25% scale
                Assert.AreEqual(0.25f, editor.RenderScale);
            }
        }

        [TestMethod]
        public async Task ExportSizeOverridesRenderScale()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                editor.RenderScale = 0.1f;

                using (var output = editor.ExportToCanvasBitmap(512))
                {
                    await BitmapAssert.VerifyAsync(output, "Barn_exposure_thumbnail.jpg", 1.8f);
                }

                // ensure that configured render scale is retained
                Assert.AreEqual(0.1f, editor.RenderScale);
            }
        }

        [TestMethod]
        public async Task ExportEnablesAdjustmentIfDisabled()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                editor.AdjustmentEnabled = false;

                using (var output = editor.ExportToCanvasBitmap(512))
                {
                    await BitmapAssert.VerifyAsync(output, "Barn_exposure_thumbnail.jpg", 1.8f);
                }

                Assert.IsTrue(editor.AdjustmentEnabled);
            }
        }

        [TestMethod]
        public async Task SavesThumbnailToSidecar()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                await editor.SaveAsync();
            }

            var file = await sidecar.TryGetFileAsync();
            Assert.IsNotNull(file);
            await BitmapAssert.VerifyAsync(file, "Barn_exposure_thumbnail.jpg");
        }

        [TestMethod]
        public async Task LoadsPreviouslySavedAdjustment()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                await editor.SaveAsync();
            }

            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                Assert.AreEqual(0.5f, editor.Adjustment.Exposure);
            }
        }

        [TestMethod]
        public async Task DoesNotWriteSidecarWhenNoAdjustmentIsMade()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                // the "meta" properties below are not serialized, make
                // sure that changing them does not create a sidecar
                editor.AdjustmentEnabled = false;
                editor.RenderScale = 0.5f;

                await editor.SaveAsync();
            }

            Assert.IsNull(await sidecar.TryGetFileAsync());
        }

        [TestMethod]
        public async Task DeletesSidecarWhenAdjustmentIsReset()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                await editor.SaveAsync();
            }

            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.ResetAdjustment();
                await editor.SaveAsync();
            }

            Assert.IsNull(await sidecar.TryGetFileAsync());
        }

        [TestMethod]
        public void ScalesSizeToFitMaxDimension()
        {
            Assert.AreEqual(new Size(100, 25), PhotoEditor.ScaleDimensions(new Size(200, 50), 100), "width > maxDimension: should scale width to maxDimension");
            Assert.AreEqual(new Size(25, 100), PhotoEditor.ScaleDimensions(new Size(50, 200), 100), "height > maxDimension: should scale height to maxDimension");
            Assert.AreEqual(new Size(100, 50), PhotoEditor.ScaleDimensions(new Size(100, 50), 200), "originalSize < maxDimension: should not scale");
        }

        private async Task VerifyOutputAsync(PhotoEditor editor, string filename)
        {
            using (var output = editor.ExportToCanvasBitmap())
            {
                await BitmapAssert.VerifyAsync(output, filename);
            }
        }
    }
}
