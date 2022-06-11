using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix
{
    [TestClass]
    public class PhotoMetadataTest
    {
        [TestMethod]
        public async Task RetrievesMetadataFromStorageFile()
        {
            var file = await TestData.GetFileAsync("Photos\\Snow.jpg");
            var metadata = await PhotoMetadata.CreateFromStorageItemAsync(file.Properties);

            Assert.AreEqual("Snow.jpg", metadata.FileName);
            Assert.AreEqual(new DateTime(2014, 2, 8, 20, 46, 28), metadata.CaptureDate);

            Assert.AreEqual("Canon", metadata.CameraManufacturer);
            Assert.AreEqual("Canon EOS REBEL T2i", metadata.CameraModel);

            Assert.AreEqual(900, metadata.ImageWidth);
            Assert.AreEqual(600, metadata.ImageHeight);

            Assert.AreEqual(18, metadata.FocalLength);
            Assert.AreEqual(4.5, metadata.FNumber);
            Assert.AreEqual(0.2, metadata.ExposureTime);
            Assert.AreEqual(800, metadata.ISOSpeed);
        }
    }
}
