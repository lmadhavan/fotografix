using Fotografix.IO;
using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fotografix.Tests
{
    [TestClass]
    public class WindowsImageDecoderTest
    {
        private IImageDecoder decoder;

        [TestMethod]
        public void SupportsStandardFileFormats()
        {
            this.decoder = new WindowsImageDecoder();
            AssertFileFormat("JPEG", ".jpg");
            AssertFileFormat("PNG", ".png");
            AssertFileFormat("GIF", ".gif");
            AssertFileFormat("TIFF", ".tif");
            AssertFileFormat("BMP", ".bmp");
        }

        private void AssertFileFormat(string name, string fileExtension)
        {
            foreach (FileFormat format in decoder.SupportedFileFormats)
            {
                if (format.Name == name)
                {
                    CollectionAssert.Contains(format.FileExtensions.ToList(), fileExtension);
                    return;
                }
            }

            Assert.Fail("File format not found: " + name);
        }
    }
}
