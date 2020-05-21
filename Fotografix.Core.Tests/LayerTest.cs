using Fotografix.Testing;
using NUnit.Framework;

namespace Fotografix.Core.Tests
{
    [TestFixture]
    public class LayerTest
    {
        private Layer layer;

        [SetUp]
        public void SetUp()
        {
            this.layer = new FakeLayer();
        }

        [Test]
        public void ChangingVisibilityRaisesContentChanged()
        {
            Assert.That(layer, Raises.ContentChanged.When(() => layer.Visible = false));
        }

        [Test]
        public void ChangingOpacityRaisesContentChanged()
        {
            Assert.That(layer, Raises.ContentChanged.When(() => layer.Opacity = 0.5f));
        }

        [Test]
        public void ChangingBlendModeRaisesContentChanged()
        {
            Assert.That(layer, Raises.ContentChanged.When(() => layer.BlendMode = BlendMode.Multiply));
        }
    }
}
