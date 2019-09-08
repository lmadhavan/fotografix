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

        protected async Task<CanvasBitmap> LoadBitmapAsync(string filename)
        {
            var file = await imagesFolder.GetFileAsync(filename);

            using (var stream = await file.OpenReadAsync())
            {
                return await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
            }
        }

        protected async Task<Image> LoadImageAsync(string filename)
        {
            var file = await imagesFolder.GetFileAsync(filename);
            return await Image.LoadAsync(file);
        }
    }
}