using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.UI.FileManagement
{
    public sealed class OpenFileCommand : ICreateImageEditorCommand
    {
        private readonly StorageFile file;

        public OpenFileCommand(StorageFile file)
        {
            this.file = file;
        }

        public string Title => file.DisplayName;

        public async Task<ImageEditor> ExecuteAsync()
        {
            return await ImageEditor.CreateAsync(file);
        }
    }
}