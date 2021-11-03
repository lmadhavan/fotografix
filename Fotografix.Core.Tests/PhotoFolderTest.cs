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
            var photos = await photoFolder.GetPhotosAsync();

            Assert.AreEqual(3, photos.Count);
            Assert.AreEqual("Barn.jpg", photos[0].Name);
            Assert.AreEqual("Snow.jpg", photos[1].Name);
            Assert.AreEqual("ZZZ_BadImage.jpg", photos[2].Name);
        }
    }
}
