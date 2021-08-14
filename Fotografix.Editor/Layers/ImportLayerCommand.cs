using Fotografix.Editor.FileManagement;
using Fotografix.IO;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    public sealed class ImportLayerCommand : DocumentCommand
    {
        private readonly ImportProcessor processor;

        public ImportLayerCommand(IImageDecoder imageDecoder, IFilePicker filePicker)
        {
            this.processor = new(imageDecoder, filePicker);
        }

        public override Task ExecuteAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            return processor.ProcessBatchAsync(document, parameter, cancellationToken, progress);
        }

        private sealed class ImportProcessor : BatchImageProcessor<Document>
        {
            public ImportProcessor(IImageDecoder imageDecoder, IFilePicker filePicker) : base(imageDecoder, filePicker)
            {
            }

            protected override string GetProgressDescription(IFile file)
            {
                return "Importing " + file.Name;
            }

            protected async override Task ProcessAsync(Document document, IFile file)
            {
                Image importedImage = await ReadImageAsync(file);
                foreach (Layer layer in importedImage.DetachLayers())
                {
                    document.Image.Layers.Add(layer);
                }
            }
        }
    }
}
