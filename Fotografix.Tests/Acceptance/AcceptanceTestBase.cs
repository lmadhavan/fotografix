using Fotografix.Editor;
using Fotografix.Editor.Clipboard;
using Fotografix.IO;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    public abstract class AcceptanceTestBase : IDialog<ResizeImageParameters>
    {
        private readonly ImageEditorFactory imageEditorFactory;

        private bool invalidated;
        private Viewport viewport;

        protected AcceptanceTestBase()
        {
            this.Clipboard = new FakeClipboard();
            this.imageEditorFactory = new ImageEditorFactory(Clipboard) { ResizeImageDialog = this };
        }

        protected FakeClipboard Clipboard { get; }
        protected ImageEditor Editor { get; private set; }

        protected async Task OpenImageAsync(string filename)
        {
            Editor?.Dispose();

            this.viewport = new Viewport();

            var file = await GetFileAsync(filename);
            Document document = await imageEditorFactory.OpenImageAsync(file);
            this.Editor = imageEditorFactory.CreateEditor(viewport, document);
            Editor.Invalidated += Editor_Invalidated;
            Editor.PropertyChanged += Editor_PropertyChanged;

            SyncViewportSize();
        }

        protected async Task ImportLayerAsync(string filename)
        {
            var file = await GetFileAsync(filename);
            await Editor.ImportLayersAsync(new IFile[] { file });
        }

        protected async Task SaveToTempFolderAsync(string filename)
        {
            await Editor.ToBitmap().CaptureToTempFolderAsync(filename);
        }

        protected async Task AssertImageAsync(string fileWithExpectedImage)
        {
            AssertInvalidated($"Displayed image was not updated (expecting {fileWithExpectedImage})");
            await BitmapAssert.AreEquivalentAsync(fileWithExpectedImage, Editor.ToBitmap());
        }

        protected void ResizeViewport(Size size)
        {
            viewport.Size = size;
        }

        protected virtual bool HandleResizeImageDialog(ResizeImageParameters parameters)
        {
            return false;
        }

        private async Task<IFile> GetFileAsync(string filename)
        {
            var file = await TestImages.GetFileAsync(filename);
            return new StorageFileAdapter(file);
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

        Task<bool> IDialog<ResizeImageParameters>.ShowAsync(ResizeImageParameters parameters)
        {
            return Task.FromResult(HandleResizeImageDialog(parameters));
        }
    }
}