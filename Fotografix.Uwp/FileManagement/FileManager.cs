using Fotografix.Editor;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp.FileManagement
{
    public sealed class FileManager
    {
        private readonly ImageEditorFactory imageEditorFactory = new ImageEditorFactory(ClipboardAdapter.GetForCurrentThread());

        public event EventHandler<OpenImageEditorRequestedEventArgs> OpenImageEditorRequested;

        public async Task NewImageAsync()
        {
            NewImageParameters parameters = new NewImageParameters();

            NewImageDialog dialog = new NewImageDialog(parameters);
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                Document document = imageEditorFactory.CreateNewImage(parameters.Size);
                OpenImageEditor(document);
            }
        }

        public async Task OpenFileAsync()
        {
            FileOpenPicker picker = FilePickerFactory.CreateFileOpenPicker(imageEditorFactory.SupportedOpenFormats);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await OpenFileAsync(file);
            }
        }

        public async Task OpenRecentFileAsync(RecentFile recentFile)
        {
            StorageFile file = await RecentFileList.Default.GetFileAsync(recentFile);
            await OpenFileAsync(file);
        }

        private async Task OpenFileAsync(StorageFile file)
        {
            Document document = await imageEditorFactory.OpenImageAsync(new StorageFileAdapter(file));
            RecentFileList.Default.Add(file);
            OpenImageEditor(document);
        }

        private void OpenImageEditor(Document document)
        {
            CreateImageEditorFunc createFunc = viewport => imageEditorFactory.CreateEditor(viewport, document);
            OpenImageEditorRequested?.Invoke(this, new OpenImageEditorRequestedEventArgs(createFunc));
        }
    }
}
