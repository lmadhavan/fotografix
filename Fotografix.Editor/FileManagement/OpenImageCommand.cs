using Fotografix.IO;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public sealed class OpenImageCommand : EditorCommand
    {
        private readonly IImageDecoder imageDecoder;
        private readonly IFilePicker filePicker;

        public OpenImageCommand(IImageDecoder imageDecoder, IFilePicker filePicker)
        {
            this.imageDecoder = imageDecoder;
            this.filePicker = filePicker;
        }

        public async override Task ExecuteAsync(Workspace workspace, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            var files = (await filePicker.PickOpenFilesAsync(imageDecoder.SupportedFileFormats)).ToList();

            for (int i = 0; i < files.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var file = files[i];
                progress?.Report(new("Opening " + file.Name, i, files.Count));

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
