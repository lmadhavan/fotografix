using Fotografix.Editor.FileManagement;
using Fotografix.IO;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    public sealed class ImportLayerCommand : DocumentCommand
    {
        private readonly IImageDecoder imageDecoder;
        private readonly IFilePicker filePicker;

        public ImportLayerCommand(IImageDecoder imageDecoder, IFilePicker filePicker)
        {
            this.imageDecoder = imageDecoder;
            this.filePicker = filePicker;
        }

        public async override Task ExecuteAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            var files = (await filePicker.PickOpenFilesAsync(imageDecoder.SupportedFileFormats)).ToList();

            for (int i = 0; i < files.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var file = files[i];
                progress?.Report(new("Importing " + file.Name, i, files.Count));

                Image importedImage = await imageDecoder.ReadImageAsync(file);
                foreach (Layer layer in importedImage.DetachLayers())
                {
                    document.Image.Layers.Add(layer);
                }
            }
        }
    }
}
