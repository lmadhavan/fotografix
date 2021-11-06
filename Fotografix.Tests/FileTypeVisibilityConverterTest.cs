using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml;

namespace Fotografix
{
    [TestClass]
    public class FileTypeVisibilityConverterTest
    {
        [TestMethod]
        public void VisibleOnlyForSpecifiedFileTypes()
        {
            var converter = new FileTypeVisibilityConverter { VisibleFileTypes = ".jpg;.jpeg" };

            Assert.AreEqual(Visibility.Visible, converter.Convert("test.jpg", null, null, null));
            Assert.AreEqual(Visibility.Visible, converter.Convert("test.jpeg", null, null, null));
            Assert.AreEqual(Visibility.Collapsed, converter.Convert("test.png", null, null, null));
        }
    }
}
