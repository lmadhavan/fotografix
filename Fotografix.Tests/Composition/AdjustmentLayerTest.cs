using Fotografix.Adjustments;
using Fotografix.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class AdjustmentLayerTest
    {
        [TestMethod]
        public void InvalidatesOnPropertyChange()
        {
            HueSaturationAdjustment adjustment = new HueSaturationAdjustment();
            AdjustmentLayer layer = new AdjustmentLayer(adjustment);

            bool invalidated = false;
            layer.Invalidated += (s, e) => invalidated = true;

            adjustment.Hue = 1;
            Assert.IsTrue(invalidated);
        }
    }
}
