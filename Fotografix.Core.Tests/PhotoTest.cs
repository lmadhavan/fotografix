using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix
{
    [TestClass]
    public class PhotoTest
    {
        private StorageFile content;

        [TestInitialize]
        public async Task Initialize()
        {
            this.content = await TestData.GetFileAsync("Photos\\Barn.jpg");
        }

        [TestMethod]
        public async Task ReturnsDefaultThumbnailWhenSidecarDoesNotExist()
        {
            var sidecar = new StorageFileReference(ApplicationData.Current.TemporaryFolder, "non-existent-file");
            var photo = new Photo(content, sidecar);

            using (var thumbnail = await photo.GetThumbnailAsync())
            {
                await BitmapAssert.VerifyAsync(thumbnail, "Barn_thumbnail.jpg");
            }
        }

        [TestMethod]
        public async Task ReturnsThumbnailFromSidecarWhenAvailable()
        {
            var sidecar = await TestData.GetFileReferenceAsync("Barn_exposure_thumbnail.jpg");
            var photo = new Photo(content, sidecar);

            using (var thumbnail = await photo.GetThumbnailAsync())
            {
                await BitmapAssert.VerifyAsync(thumbnail, "Barn_exposure_thumbnail.jpg");
            }
        }
    }
}
