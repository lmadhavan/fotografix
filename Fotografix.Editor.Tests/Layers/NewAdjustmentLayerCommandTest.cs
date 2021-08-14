using Fotografix.Adjustments;
using NUnit.Framework;

namespace Fotografix.Editor.Layers
{
    [TestFixture]
    public class NewAdjustmentLayerCommandTest
    {
        private NewAdjustmentLayerCommand command;
        private Document document;

        [SetUp]
        public void SetUp()
        {
            this.command = new();
            this.document = new();
        }

        [Test]
        public void CanExecuteWithAdjustmentType()
        {
            Assert.IsTrue(command.CanExecute(document, typeof(BlackAndWhiteAdjustment)));
        }

        [Test]
        public void CannotExecuteWithNonAdjustmentType()
        {
            Assert.IsFalse(command.CanExecute(document, typeof(Bitmap)));
        }

        [Test]
        public void CanExecuteWithNullParameter()
        {
            Assert.IsFalse(command.CanExecute(document, null));
        }
        
        [Test]
        public void AddsAdjustmentLayerOfSpecifiedType()
        {
            command.Execute(document, typeof(BlackAndWhiteAdjustment));

            var layers = document.Image.Layers;
            Assert.That(layers.Count, Is.EqualTo(1));
            Assert.That(layers[0].Content, Is.InstanceOf<BlackAndWhiteAdjustment>());
        }
    }
}
