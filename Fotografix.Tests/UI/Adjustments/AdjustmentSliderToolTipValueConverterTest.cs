using Fotografix.UI.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Tests.UI.Adjustments
{
    [TestClass]
    public class AdjustmentSliderToolTipValueConverterTest
    {
        [TestMethod]
        public void FormatsAdjustmentValue()
        {
            AdjustmentSliderToolTipValueConverter converter = new AdjustmentSliderToolTipValueConverter();
            Assert.AreEqual("0.00", converter.Convert(0.0, null, null, null));
            Assert.AreEqual("+0.50", converter.Convert(0.5, null, null, null));
            Assert.AreEqual("-0.50", converter.Convert(-0.5, null, null, null));
        }
    }
}
