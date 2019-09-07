using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Fotografix.Editor.Tests
{
    [TestClass]
    public class ImageTest
    {
        [TestMethod]
        public async Task LoadsImageFromFile()
        {
            var imagesFolder = await Package.Current.InstalledLocation.GetFolderAsync("Images");
            var flowers = await imagesFolder.GetFileAsync("flowers.jpg");

            using (Image image = await Image.LoadAsync(flowers))
            {
                Assert.AreEqual(320, image.Width);
                Assert.AreEqual(480, image.Height);
            }
        }
    }
}
