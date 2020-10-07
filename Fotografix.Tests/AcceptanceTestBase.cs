using Fotografix.Editor;
using Fotografix.Editor.Testing;
using Fotografix.UI;
using Fotografix.Win2D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Fotografix.Tests
{
    public abstract class AcceptanceTestBase
    {
        private bool invalidated;

        protected ImageEditor Editor { get; private set; }
        protected Viewport Viewport { get; private set; }

        protected async Task OpenImageAsync(string filename)
        {
            Editor?.Dispose();

            this.Viewport = new FakeViewport();

            var file = await TestImages.GetFileAsync(filename);
            this.Editor = await ImageEditor.CreateAsync(new StorageFileAdapter(file), Viewport, new WindowsImageDecoder(), new WindowsImageEncoder(new Win2DImageRenderer()));
            Editor.Invalidated += OnEditorInvalidated;
        }

        protected async Task SaveToTempFolderAsync(string filename)
        {
            await Editor.ToBitmap().CaptureToTempFolderAsync(filename);
        }

        protected async Task AssertImageAsync(string fileWithExpectedImage)
        {
            AssertInvalidated($"Displayed image was not updated (expecting {fileWithExpectedImage})");
            await AssertImage.IsEquivalentAsync(fileWithExpectedImage, Editor.ToBitmap());
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