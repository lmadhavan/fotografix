using Fotografix.Adjustments;
using NUnit.Framework;

namespace Fotografix.Core.Tests
{
    [TestFixture]
    public class AdjustmentLayerTest
    {
        private HueSaturationAdjustment adjustment;
        private Layer layer;

        [SetUp]
        public void SetUp()
        {
            this.adjustment = new HueSaturationAdjustment();
            this.layer = new AdjustmentLayer(adjustment);
        }

        [Test]
        public void ChangingAdjustmentPropertyRaisesContentChanged()
        {
            Assert.That(layer, Raises.ContentChanged.When(() => adjustment.Hue = 0.5f));
        }
    }
}
