using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Editor.UI.Tests
{
    [TestClass]
    public class ImageEditorViewModelTest
    {
        private ImageEditorViewModel viewModel;
        private Layer layer;

        [TestInitialize]
        public void Initialize()
        {
            this.viewModel = new ImageEditorViewModel(new Image(CanvasDevice.GetSharedDevice(), 1, 1));
            this.layer = new AdjustmentLayer(new BlackAndWhiteAdjustment());
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
            Assert.AreEqual(layer, viewModel.Layers[1]);
            Assert.AreEqual(layer, viewModel.SelectedLayer);
        }

        [TestMethod]
        public void DeletesAdjustment()
        {
            Assert.IsFalse(viewModel.DeleteLayerCommand.CanExecute(null), "Command should be initially disabled");

            viewModel.AddLayer(layer);

            Assert.IsTrue(viewModel.DeleteLayerCommand.CanExecute(null), "Command should be enabled after adding layer");

            viewModel.DeleteLayerCommand.Execute(null);

            Assert.AreEqual(1, viewModel.Layers.Count);
            Assert.AreEqual(viewModel.Layers[0], viewModel.SelectedLayer);

            Assert.IsFalse(viewModel.DeleteLayerCommand.CanExecute(null), "Command should be disabled after deleting layer");
        }

        [TestMethod]
        public void BlendModeNotEnabledOnBottomLayer()
        {
            viewModel.AddLayer(layer);

            viewModel.SelectedLayer = viewModel.Layers[0];
            Assert.IsFalse(viewModel.IsBlendModeEnabled);

            viewModel.SelectedLayer = viewModel.Layers[1];
            Assert.IsTrue(viewModel.IsBlendModeEnabled);
        }
    }
}
