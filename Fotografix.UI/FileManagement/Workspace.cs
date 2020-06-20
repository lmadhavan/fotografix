using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI.FileManagement
{
    public sealed class Workspace
    {
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
            FileOpenPicker picker = FilePickerFactory.CreateFilePicker();

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                OpenImageEditor(new OpenFileCommand(file));
            }
        }

        private void OpenImageEditor(ICreateImageEditorCommand command)
        {
            OpenImageEditorRequested?.Invoke(this, new OpenImageEditorRequestedEventArgs(command));
        }
    }
}
