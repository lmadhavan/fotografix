using Fotografix.Editor.Commands;
using Fotografix.UI;
using Fotografix.UI.Layers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Fotografix.Tests.UI.Layers
{
    [TestClass]
    public class LayerReorderChangeSynthesizerTest
    {
        private Image image;
        private CommandService commandService;

        private LayerReorderChangeSynthesizer synthesizer;
        private IChange change;

        [TestInitialize]
        public void Initialize()
        {
            this.image = new Image(Size.Empty);
            image.Layers.Add(BitmapLayerFactory.CreateBitmapLayer("Layer 1"));
            image.Layers.Add(BitmapLayerFactory.CreateBitmapLayer("Layer 2"));

            this.commandService = new CommandService();

            this.synthesizer = new LayerReorderChangeSynthesizer(image, commandService);
            synthesizer.ChangeSynthesized += (s, e) => this.change = e.Change;
        }

        [TestMethod]
        public void RemoveLayerAndInsertImmediately_SynthesizesChange()
        {
            // when I remove a layer and insert it at another position
            Layer toBeMoved = image.Layers[0];
            image.Layers.RemoveAt(0);
            image.Layers.Insert(1, toBeMoved);

            // then a ReorderLayerChange is synthesized
            AssertReorderLayerChangeSynthesized(fromIndex: 0, toIndex: 1);
        }

        [TestMethod]
        public void RemoveLayerAndInsertDifferentLayer_DoesNotSynthesizeChange()
        {
            // when I remove a layer and insert a different layer
            image.Layers.RemoveAt(0);
            image.Layers.Insert(1, BitmapLayerFactory.CreateBitmapLayer("Layer 3"));

            // then no change is synthesized
            Assert.IsNull(change);
        }

        [TestMethod]
        public void RemoveLayerAndInsertLater_DoesNotSynthesizeChange()
        {
            // when I remove a layer, insert a different layer and then insert the original layer back
            Layer toBeMoved = image.Layers[0];
            image.Layers.RemoveAt(0);
            image.Layers.Insert(1, BitmapLayerFactory.CreateBitmapLayer("Layer 3"));
            image.Layers.Insert(2, toBeMoved);

            // then no change is synthesized
            Assert.IsNull(change);
        }

        [TestMethod]
        public void UndoAndRedo_DoesNotSynthesizeChange()
        {
            // when I undo and redo a ReorderLayerChange
            commandService.Execute(new ReorderLayerCommand(image, 0, 1));
            commandService.Undo();
            commandService.Redo();

            // then a duplicate change is not synthesized
            Assert.IsNull(change);
        }

        private void AssertReorderLayerChangeSynthesized(int fromIndex, int toIndex)
        {
            Assert.IsTrue(commandService.CanUndo);
            Assert.IsInstanceOfType(change, typeof(ReorderLayerCommand));

            ReorderLayerCommand reorderLayerCommand = (ReorderLayerCommand)change;
            Assert.AreEqual(fromIndex, reorderLayerCommand.FromIndex);
            Assert.AreEqual(toIndex, reorderLayerCommand.ToIndex);
        }
    }
}
