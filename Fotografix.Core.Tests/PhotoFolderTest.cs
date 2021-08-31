using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Fotografix
{
    [TestClass]
    public class PhotoFolderTest
    {
        [TestMethod]
        public async Task RetrievesPhotosInFolder()
        {
            var folder = await Package.Current.InstalledLocation.GetFolderAsync("Test Photos");

            var photos = await PhotoFolder.GetPhotosAsync(folder);

            Assert.AreEqual(2, photos.Count);
            Assert.AreEqual("Barn.jpg", photos[0].Name);
            Assert.AreEqual("Snow.jpg", photos[1].Name);
        }
    }
}
