﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void ConvertsBlendModeIndexToEnumValue()
        {
            int index = viewModel.BlendModes.IndexOf("Multiply");
            Assert.AreNotEqual(-1, index);

            viewModel.SelectedBlendModeIndex = index;
            Assert.AreEqual(BlendMode.Multiply, viewModel.SelectedBlendMode);
        }
    }
}
