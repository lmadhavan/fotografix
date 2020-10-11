using Fotografix.Uwp.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Tests.Uwp.Adjustments
{
    [TestClass]
    public class AdjustmentSliderToolTipValueConverterTest
    {
        [TestMethod]
        public void FormatsAdjustmentValue()
        {
            AdjustmentSliderToolTipValueConverter converter = new AdjustmentSliderToolTipValueConverter();
            Assert.AreEqual("0.00", converter.Convert(0.0));
            Assert.AreEqual("+0.50", converter.Convert(0.5));
            Assert.AreEqual("-0.50", converter.Convert(-0.5));
        }
    }
}
