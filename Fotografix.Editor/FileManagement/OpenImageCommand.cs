using Fotografix.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public sealed class OpenImageCommand : IWorkspaceCommand
    {
        private readonly IImageDecoder imageDecoder;
        private readonly IFilePicker filePicker;

        public OpenImageCommand(IImageDecoder imageDecoder, IFilePicker filePicker)
        {
            this.imageDecoder = imageDecoder;
            this.filePicker = filePicker;
        }

        public async Task ExecuteAsync(Workspace workspace)
        {
            var files = await filePicker.PickOpenFilesAsync(imageDecoder.SupportedFileFormats);

            foreach (var file in files)
            {
                Document existingDocument = workspace.Documents.FirstOrDefault(d => file.Equals(d.File));
                if (existingDocument != null)
                {
                    workspace.ActiveDocument = existingDocument;
                    continue;
                }

                Image image = await imageDecoder.ReadImageAsync(file);
                Document document = new Document(image) { File = file };
                workspace.AddDocument(document);
            }
        }
    }
}
