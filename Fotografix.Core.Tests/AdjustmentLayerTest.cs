using Fotografix.Testing;
using NUnit.Framework;

namespace Fotografix.Core.Tests
{
    [TestFixture]
    public class AdjustmentLayerTest
    {
        private FakeAdjustment adjustment;
        private Layer layer;

        [SetUp]
        public void SetUp()
        {
            this.adjustment = new FakeAdjustment();
            this.layer = new AdjustmentLayer(adjustment);
        }

        [Test]
        public void ChangingAdjustmentPropertyRaisesContentChanged()
        {
            Assert.That(layer, Raises.ContentChanged.When(() => adjustment.Property = 5));
        }
    }
}
