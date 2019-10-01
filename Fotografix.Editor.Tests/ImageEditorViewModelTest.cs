using Fotografix.Editor.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fotografix.Editor.Tests
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
        public void MapsBlendModeEnumValueToDisplayString()
        {
            int index = viewModel.BlendModes.IndexOf("Soft Light");
            Assert.AreNotEqual(-1, index);

            viewModel.AddAdjustment(new BlackAndWhiteAdjustment());
            viewModel.SelectedBlendModeIndex = index;
            Assert.AreEqual(BlendMode.SoftLight, viewModel.SelectedAdjustment.BlendMode);
        }
    }
}
