using Fotografix.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Fotografix
{
    [TestFixture]
    public class ImageTest
    {
        private Image image;
        private ContentChangedEventArgs contentChangedEvent;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(new Size(10, 10));
            image.ContentChanged += (s, e) => contentChangedEvent = e;
        }

        [Test]
        public void RaisesContentChangedEventWhenLayerAdded()
        {
            Layer layer = new Layer();

            image.Layers.Add(layer);

            Assert.That(contentChangedEvent.Change, Is.EqualTo(new AddItemChange<Layer>(image.Layers, 0, layer)));
        }

        [Test]
        public void RaisesContentChangedEventWhenLayerRemoved()
        {
            Layer layer = new Layer();

            image.Layers.Add(layer);
            image.Layers.Remove(layer);

            Assert.That(contentChangedEvent.Change, Is.EqualTo(new RemoveItemChange<Layer>(image.Layers, 0, layer)));
        }

        [Test]
        public void RaisesContentChangedEventWhenLayerReplaced()
        {
            Layer layer1 = new Layer();
            Layer layer2 = new Layer();

            image.Layers.Add(layer1);
            image.Layers[0] = layer2;

            Assert.That(contentChangedEvent.Change, Is.EqualTo(new ReplaceItemChange<Layer>(image.Layers, 0, layer1, layer2)));
        }

        [Test]
        public void RaisesContentChangedEventWhenLayerMoved()
        {
            Layer layer1 = new Layer();
            Layer layer2 = new Layer();

            image.Layers.Add(layer1);
            image.Layers.Add(layer2);
            image.Layers.Move(0, 1);

            Assert.That(contentChangedEvent.Change, Is.EqualTo(new MoveItemChange<Layer>(image.Layers, 0, 1)));
        }

        [Test]
        public void ThrowsExceptionWhenAddingLayerAttachedToAnotherImage()
        {
            Layer layer = new Layer();

            Image anotherImage = new Image(new Size(20, 20));
            anotherImage.Layers.Add(layer);

            Assert.Throws<InvalidOperationException>(() => image.Layers.Add(layer));
        }

        [Test]
        public void DetachesLayerWhenRemoved()
        {
            Layer layer = new Layer();
            image.Layers.Add(layer);
            image.Layers.Remove(layer);

            Assert.That(layer.Parent, Is.Null);
        }

        [Test]
        public void DetachesLayerWhenReplaced()
        {
            Layer layer1 = new Layer();
            Layer layer2 = new Layer();

            image.Layers.Add(layer1);
            image.Layers[0] = layer2;

            Assert.That(layer1.Parent, Is.Null);
        }

        [Test]
        public void DetachesAndReturnsAllLayers()
        {
            Layer layer1 = new Layer();
            Layer layer2 = new Layer();

            image.Layers.Add(layer1);
            image.Layers.Add(layer2);

            IList<Layer> detachedLayers = image.DetachLayers();
            Assert.That(detachedLayers, Is.EqualTo(new List<Layer> { layer1, layer2 }));
            Assert.That(layer1.Parent, Is.Null);
            Assert.That(layer2.Parent, Is.Null);
        }
    }
}
