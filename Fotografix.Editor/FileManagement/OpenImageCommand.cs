using Fotografix.IO;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    public sealed class OpenImageCommand : EditorCommand
    {
        private readonly OpenProcessor processor;

        public OpenImageCommand(IImageDecoder imageDecoder, IFilePicker filePicker)
        {
            this.processor = new(imageDecoder, filePicker);
        }

        public override Task ExecuteAsync(Workspace workspace, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            return processor.ProcessBatchAsync(workspace, parameter, cancellationToken, progress);
        }

        private sealed class OpenProcessor : BatchImageProcessor<Workspace>
        {
            public OpenProcessor(IImageDecoder imageDecoder, IFilePicker filePicker) : base(imageDecoder, filePicker)
            {
            }

            protected override string GetProgressDescription(IFile file)
            {
                return "Opening " + file.Name;
            }

            protected async override Task ProcessAsync(Workspace workspace, IFile file)
            {
                Document existingDocument = workspace.Documents.FirstOrDefault(d => file.Equals(d.File));
                if (existingDocument != null)
                {
                    workspace.ActiveDocument = existingDocument;
                    return;
                }

                Image image = await ReadImageAsync(file);
                Document document = new Document(image) { File = file };
                workspace.AddDocument(document);
            }
        }
    }
}
