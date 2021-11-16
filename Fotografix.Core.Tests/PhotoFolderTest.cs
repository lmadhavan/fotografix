using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix
{
    [TestClass]
    public class PhotoFolderTest
    {
        [TestMethod]
        public async Task RetrievesPhotosInFolder()
        {
            var folder = await TestData.GetFolderAsync("Photos");

            var photoFolder = new PhotoFolder(folder, folder);
            var photos = await photoFolder.GetPhotosAsync(new string[] { ".jpg" });

            Assert.AreEqual(3, photos.Count);
            Assert.AreEqual("Barn.jpg", photos[0].Name);
            Assert.AreEqual("Snow.jpg", photos[1].Name);
            Assert.AreEqual("ZZZ_BadImage.jpg", photos[2].Name);
        }

        [TestMethod]
        public async Task LinksRawAndJpegFiles()
        {
            var folder = await TestData.GetFolderAsync("Photos");

            var photoFolder = new PhotoFolder(folder, folder);
            var photos = await photoFolder.GetPhotosAsync(new string[] { ".jpg", ".raw" });

            Assert.AreEqual(3, photos.Count);
            Assert.AreEqual("Barn.raw", photos[0].Name);
            Assert.AreEqual(1, photos[0].LinkedPhotos.Count);
            Assert.AreEqual("Barn.jpg", photos[0].LinkedPhotos[0].Name);
        }
    }
}
