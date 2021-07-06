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
                OpenImageEditor(new NewImageCommand(parameters.Size, imageEditorFactory));
            }
        }

        public async Task OpenFileAsync()
        {
            FileOpenPicker picker = FilePickerFactory.CreateFileOpenPicker(imageEditorFactory.SupportedOpenFormats);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                OpenFile(file);
            }
        }

        public async Task OpenRecentFileAsync(RecentFile recentFile)
        {
            StorageFile file = await RecentFileList.Default.GetFileAsync(recentFile);
            OpenFile(file);
        }

        public void OpenFile(StorageFile file)
        {
            OpenImageEditor(new OpenFileCommand(new StorageFileAdapter(file), imageEditorFactory));
            RecentFileList.Default.Add(file);
        }

        private void OpenImageEditor(ICreateImageEditorCommand command)
        {
            OpenImageEditorRequested?.Invoke(this, new OpenImageEditorRequestedEventArgs(command));
        }
    }
}
