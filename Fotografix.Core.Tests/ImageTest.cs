using Fotografix.Testing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Core.Tests
{
    [TestFixture]
    public class ImageTest
    {
        private Image image;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(new Size(10, 10));
        }

        [Test]
        public void ChangingImageSizeRaisesContentChanged()
        {
            Assert.That(image, Raises.ContentChanged.When(() => image.Size = new Size(20, 20)));
        }

        [Test]
        public void AddingLayerRaisesContentChanged()
        {
            Assert.That(image, Raises.ContentChanged.When(() => image.Layers.Add(new FakeLayer())));
        }

        [Test]
        public void RemovingLayerRaisesContentChanged()
        {
            image.Layers.Add(new FakeLayer());

            Assert.That(image, Raises.ContentChanged.When(() => image.Layers.RemoveAt(0)));
        }

        [Test]
        public void ReorderingLayersRaisesContentChanged()
        {
            image.Layers.Add(new FakeLayer());
            image.Layers.Add(new FakeLayer());

            Assert.That(image, Raises.ContentChanged.When(() => image.Layers.Move(0, 1)));
        }

        [Test]
        public void ClearingLayersRaisesContentChanged()
        {
            image.Layers.Add(new FakeLayer());
            image.Layers.Add(new FakeLayer());

            Assert.That(image, Raises.ContentChanged.When(() => image.Layers.Clear()));
        }

        [Test]
        public void ForwardsContentChangedEventFromLayers()
        {
            FakeLayer layer = new FakeLayer();
            image.Layers.Add(layer);

            Assert.That(image, Raises.ContentChanged.When(() => layer.RaiseContentChanged()));
        }
    }
}
