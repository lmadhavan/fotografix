using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI.FileManagement
{
    public sealed class Workspace
    {
        public RecentFileList RecentFiles { get; } = new RecentFileList(StorageApplicationPermissions.MostRecentlyUsedList);
        
        public event EventHandler<OpenImageEditorRequestedEventArgs> OpenImageEditorRequested;

        public async Task NewImageAsync()
        {
            NewImageParameters parameters = new NewImageParameters();

            NewImageDialog dialog = new NewImageDialog(parameters);
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                OpenImageEditor(new NewImageCommand(parameters.Size));
            }
        }

        public async Task OpenFileAsync()
        {
            FileOpenPicker picker = FilePickerFactory.CreateFileOpenPicker();

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                OpenFile(file);
            }
        }

        public async Task OpenRecentFileAsync(RecentFile recentFile)
        {
            StorageFile file = await RecentFiles.GetFileAsync(recentFile);
            OpenFile(file);
        }

        public void OpenFile(StorageFile file)
        {
            OpenImageEditor(new OpenFileCommand(file));
            RecentFiles.Add(file);
        }

        private void OpenImageEditor(ICreateImageEditorCommand command)
        {
            OpenImageEditorRequested?.Invoke(this, new OpenImageEditorRequestedEventArgs(command));
        }
    }
}
