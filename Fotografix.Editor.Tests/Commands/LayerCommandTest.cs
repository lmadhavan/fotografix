using Fotografix.Editor.Commands;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tests.Commands
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
            IChange change = new AddLayerCommand(image, newLayer).PrepareChange();

            change.Apply();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer, newLayer }));

            change.Undo();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer }));
        }

        [Test]
        public void RemoveLayer()
        {
            IChange change = new RemoveLayerCommand(image, existingLayer).PrepareChange();

            change.Apply();
            Assert.That(image.Layers, Is.Empty);

            change.Undo();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer }));
        }

        [Test]
        public void ReorderLayer()
        {
            image.Layers.Add(newLayer);

            IChange change = new ReorderLayerCommand(image, 0, 1).PrepareChange();

            change.Apply();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { newLayer, existingLayer }));

            change.Undo();
            Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer, newLayer }));
        }

        private static Layer CreateLayer()
        {
            return new BitmapLayer(Bitmap.Empty);
        }
    }
}