using Fotografix.Editor.Layers;
using Fotografix.Testing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tests
{
    public class LayerCommandTest
    {
        private Layer existingLayer;
        private Layer newLayer;
        private Image image;

        [SetUp]
        public void SetUp()
        {
            this.existingLayer = CreateLayer();
            this.newLayer = CreateLayer();

            this.image = new Image(new Size(10, 10));
            image.Layers.Add(existingLayer);
        }

        [Test]
        public void AddLayer()
        {
            Command command = new AddLayerCommand(image, newLayer);

            command.Execute();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer, newLayer }));

            command.Undo();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer }));
        }

        [Test]
        public void RemoveLayer()
        {
            Command command = new RemoveLayerCommand(image, existingLayer);

            command.Execute();
            Assert.That(image.Layers, Is.Empty);

            command.Undo();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer }));
        }

        [Test]
        public void ReorderLayer()
        {
            image.Layers.Add(newLayer);

            Command command = new ReorderLayerCommand(image, 0, 1);

            command.Execute();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { newLayer, existingLayer }));

            command.Undo();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer, newLayer }));
        }

        private static Layer CreateLayer()
        {
            return new BitmapLayer(new FakeBitmap());
        }
    }
}