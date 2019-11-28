using NUnit.Framework;
using System;
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
            AddLayerCommand command = new AddLayerCommand(image, newLayer);

            AssertCommand(
                command,
                preCondition: () => Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer })),
                postCondition: () => Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer, newLayer }))
            );
        }

        [Test]
        public void RemoveLayer()
        {
            RemoveLayerCommand command = new RemoveLayerCommand(image, existingLayer);

            AssertCommand(
                command,
                preCondition: () => Assert.That(image.Layers, Is.EqualTo(new Layer[] { existingLayer })),
                postCondition: () => Assert.That(image.Layers, Is.Empty)
            );
        }

        private static void AssertCommand(ICommand command, Action preCondition, Action postCondition)
        {
            command.Execute();

            postCondition();

            command.Undo();

            preCondition();

            command.Redo();

            postCondition();
        }

        private static Layer CreateLayer()
        {
            return new BitmapLayer(Bitmap.Empty);
        }
    }
}