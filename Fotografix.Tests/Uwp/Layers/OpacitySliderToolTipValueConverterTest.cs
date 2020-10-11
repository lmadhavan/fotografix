using Fotografix.Uwp.Layers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Tests.Uwp.Layers
{
    [TestClass]
    public class OpacitySliderToolTipValueConverterTest
    {
        [TestMethod]
        public void FormatsOpacityValue()
        {
            OpacitySliderToolTipValueConverter converter = new OpacitySliderToolTipValueConverter();
            Assert.AreEqual("0%", converter.Convert(0.0));
            Assert.AreEqual("100%", converter.Convert(1.0));
        }
    }
}
