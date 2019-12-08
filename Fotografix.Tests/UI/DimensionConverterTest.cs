using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class DimensionConverterTest
    {
        private DimensionConverter converter = new DimensionConverter();

        [TestMethod]
        public void ConvertsIntToString()
        {
            Assert.AreEqual("5", converter.Convert(5, null, null, null));
        }

        [TestMethod]
        public void ConvertsStringBackToInt()
        {
            Assert.AreEqual(5, converter.ConvertBack("5", null, null, null));
        }

        [TestMethod]
        public void CoercesZeroValueToOne()
        {
            Assert.AreEqual(1, converter.ConvertBack("0", null, null, null));
        }

        [TestMethod]
        public void CoercesNegativeValueToOne()
        {
            Assert.AreEqual(1, converter.ConvertBack("-5", null, null, null));
        }

        [TestMethod]
        public void CoercesInvalidValueToOne()
        {
            Assert.AreEqual(1, converter.ConvertBack("foo", null, null, null));
        }
    }
}
