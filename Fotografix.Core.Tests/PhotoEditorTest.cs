using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
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
            var folder = await TestData.GetFolderAsync("Photos");
            var content = await folder.GetFileAsync("Barn.jpg");
            this.sidecar = new StorageFileReference(ApplicationData.Current.TemporaryFolder, "Barn.dat");

            this.photo = new Photo(content, sidecar);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await sidecar.DeleteAsync();
        }

        [TestMethod]
        public async Task LoadsPreviouslySavedAdjustment()
        {
            using (var editor = await PhotoEditor.CreateAsync(photo))
            {
                editor.Adjustment.Exposure = 0.5f;
                await editor.SaveAsync();
            }

            Assert.IsNotNull(await sidecar.TryGetFileAsync());

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
    }
}
