using Fotografix.Adjustments;
using Fotografix.Composition;
using Fotografix.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class ImageEditorViewModelTest
    {
        private Image image;
        private Layer layer;
        private ImageEditorViewModel viewModel;

        [TestInitialize]
        public void Initialize()
        {
            this.image = new Image(CanvasDevice.GetSharedDevice(), 1, 1);
            this.layer = new AdjustmentLayer(new BlackAndWhiteAdjustment());
            this.viewModel = new ImageEditorViewModel(image);
        }

        [TestCleanup]
        public void Cleanup()
        {
            viewModel.Dispose();
            layer.Dispose();
        }

        [TestMethod]
        public void AddsLayer()
        {
            viewModel.AddLayer(layer);

            Assert.AreEqual(2, viewModel.Layers.Count);
            Assert.AreEqual(layer, viewModel.SelectedLayer);
        }

        [TestMethod]
        public void DeletesLayer()
        {
            Assert.IsFalse(viewModel.DeleteLayerCommand.CanExecute(null), "Command should be initially disabled");

            viewModel.AddLayer(layer);

            Assert.IsTrue(viewModel.DeleteLayerCommand.CanExecute(null), "Command should be enabled after adding layer");

            viewModel.DeleteLayerCommand.Execute(null);

            Assert.AreEqual(1, viewModel.Layers.Count);
            Assert.AreNotEqual(layer, viewModel.SelectedLayer);

            Assert.IsFalse(viewModel.DeleteLayerCommand.CanExecute(null), "Command should be disabled after deleting layer");
        }

        [TestMethod]
        public void DisplaysLayersInReverseOrder()
        {
            Assert.AreEqual(image.Layers[0], viewModel.Layers[0]);
         
            viewModel.AddLayer(layer);

            Assert.AreEqual(image.Layers[1], viewModel.Layers[0]);
            Assert.AreEqual(image.Layers[0], viewModel.Layers[1]);
        }

        [TestMethod]
        public void BlendModeNotEnabledOnBottomLayer()
        {
            viewModel.AddLayer(layer);

            viewModel.SelectedLayer = viewModel.Layers.Last();
            Assert.IsFalse(viewModel.IsBlendModeEnabled);

            viewModel.SelectedLayer = viewModel.Layers.First();
            Assert.IsTrue(viewModel.IsBlendModeEnabled);
        }
    }
}
