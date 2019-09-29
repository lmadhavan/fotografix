using Fotografix.Editor.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Editor.Tests
{
    [TestClass]
    public class ImageEditorViewModelTest : EditorTestBase
    {
        private ImageEditorViewModel viewModel;

        [TestInitialize]
        public async Task Initialize()
        {
            this.viewModel = new ImageEditorViewModel(await LoadImageAsync("flowers.jpg"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            viewModel.Dispose();
        }

        [TestMethod]
        public void AddsAdjustment()
        {
            viewModel.AddAdjustment(new BlackAndWhiteAdjustment());

            Assert.AreEqual(1, viewModel.Adjustments.Count);
            Assert.AreEqual(viewModel.Adjustments[0], viewModel.SelectedAdjustment);
        }

        [TestMethod]
        public void ConvertsBlendModeIndexToEnumValue()
        {
            int index = viewModel.BlendModes.IndexOf("Multiply");
            Assert.AreNotEqual(-1, index);

            viewModel.AddAdjustment(new BlackAndWhiteAdjustment());
            viewModel.SelectedBlendModeIndex = index;
            Assert.AreEqual(BlendMode.Multiply, viewModel.SelectedAdjustment.BlendMode);
        }
    }
}
