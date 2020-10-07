using Fotografix.IO;
using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Fotografix.Tests
{
    [TestClass]
    public class WindowsImageCodecTest
    {
        private IEnumerable<FileFormat> fileFormats;

        [TestMethod]
        public void DecoderSupportsStandardFileFormats()
        {
            this.fileFormats = new WindowsImageDecoder().SupportedFileFormats;
            AssertStandardFileFormats();
        }

        [TestMethod]
        public void EncoderSupportsStandardFileFormats()
        {
            this.fileFormats = new WindowsImageEncoder(new NullImageRenderer()).SupportedFileFormats;
            AssertStandardFileFormats();
        }

        private void AssertStandardFileFormats()
        {
            AssertFileFormat("JPEG", ".jpg");
            AssertFileFormat("PNG", ".png");
            AssertFileFormat("GIF", ".gif");
            AssertFileFormat("TIFF", ".tif");
            AssertFileFormat("BMP", ".bmp");
        }

        private void AssertFileFormat(string name, string fileExtension)
        {
            foreach (FileFormat format in fileFormats)
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
