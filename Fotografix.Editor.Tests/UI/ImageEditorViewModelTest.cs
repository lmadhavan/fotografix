using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Editor.UI.Tests
{
    [TestClass]
    public class ImageEditorViewModelTest
    {
        private ImageEditorViewModel viewModel;
        private Adjustment adjustment;

        [TestInitialize]
        public void Initialize()
        {
            this.viewModel = new ImageEditorViewModel(new Image(CanvasDevice.GetSharedDevice(), 1, 1));
            this.adjustment = new BlackAndWhiteAdjustment();
        }

        [TestCleanup]
        public void Cleanup()
        {
            viewModel.Dispose();
            adjustment.Dispose();
        }

        [TestMethod]
        public void AddsAdjustment()
        {
            Assert.IsFalse(viewModel.AdjustmentPropertiesVisible);

            viewModel.AddAdjustment(adjustment);

            Assert.AreEqual(1, viewModel.Adjustments.Count);
            Assert.AreEqual(adjustment, viewModel.SelectedAdjustment);
            Assert.AreEqual(adjustment, viewModel.Adjustments[0]);
            Assert.IsTrue(viewModel.AdjustmentPropertiesVisible);
        }

        [TestMethod]
        public void DeletesAdjustment()
        {
            Assert.IsFalse(viewModel.DeleteAdjustmentCommand.CanExecute(null), "Command should be initially disabled");

            viewModel.AddAdjustment(adjustment);

            Assert.IsTrue(viewModel.DeleteAdjustmentCommand.CanExecute(null), "Command should be enabled after adding adjustment");

            viewModel.DeleteAdjustmentCommand.Execute(null);

            Assert.AreEqual(0, viewModel.Adjustments.Count);
            Assert.IsNull(viewModel.SelectedAdjustment);

            Assert.IsFalse(viewModel.DeleteAdjustmentCommand.CanExecute(null), "Command should be disabled after deleting adjustment");
        }
    }
}
