using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    public abstract class AcceptanceTestBase
    {
        private bool invalidated;

        protected ImageEditor Editor { get; private set; }

        protected async Task OpenImageAsync(string filename)
        {
            Editor?.Dispose();

            var file = await TestImages.GetFileAsync(filename);
            this.Editor = await ImageEditor.CreateAsync(file);
            Editor.Invalidated += OnEditorInvalidated;
        }

        protected async Task RenderToTempFolderAsync(string filename)
        {
            await TestRenderer.RenderToTempFolderAsync(Editor, filename);
        }

        protected async Task AssertImageAsync(string fileWithExpectedImage)
        {
            AssertInvalidated($"Displayed image was not updated (expecting {fileWithExpectedImage})");
            await AssertImage.IsEquivalentAsync(fileWithExpectedImage, Editor);
        }

        protected void AssertCanUndo()
        {
            Assert.IsTrue(Editor.CanUndo);
        }

        protected void Undo()
        {
            Editor.Undo();
        }

        private void OnEditorInvalidated(object sender, EventArgs e)
        {
            this.invalidated = true;
        }

        private void AssertInvalidated(string message)
        {
            Assert.IsTrue(invalidated, message);
            this.invalidated = false;
        }
    }
}