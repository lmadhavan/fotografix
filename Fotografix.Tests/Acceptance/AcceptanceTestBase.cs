using Fotografix.Editor;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    public abstract class AcceptanceTestBase
    {
        private static readonly ImageEditorFactory ImageEditorFactory = new ImageEditorFactory();

        private bool invalidated;
        private Viewport viewport;

        protected ImageEditor Editor { get; private set; }

        protected async Task OpenImageAsync(string filename)
        {
            Editor?.Dispose();

            var file = await TestImages.GetFileAsync(filename);

            this.viewport = new Viewport();
            this.Editor = await ImageEditorFactory.OpenImageAsync(viewport, new StorageFileAdapter(file));
            Editor.Invalidated += Editor_Invalidated;
            Editor.PropertyChanged += Editor_PropertyChanged;

            SyncViewportSize();
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

        protected void Undo(int times = 1)
        {
            for (int i = 1; i <= times; i++)
            {
                Assert.IsTrue(Editor.CanUndo, $"Undo not available (${i} of ${times})");
                Editor.Undo();
            }
        }

        protected void ResizeViewport(Size size)
        {
            viewport.Size = size;
        }

        private void Editor_Invalidated(object sender, EventArgs e)
        {
            this.invalidated = true;
        }

        private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageEditor.Size))
            {
                SyncViewportSize();
            }
        }

        private void SyncViewportSize()
        {
            viewport.Size = Editor.Size;
        }

        private void AssertInvalidated(string message)
        {
            Assert.IsTrue(invalidated, message);
            this.invalidated = false;
        }
    }
}