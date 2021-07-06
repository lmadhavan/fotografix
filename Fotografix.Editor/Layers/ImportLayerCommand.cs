using Fotografix.Editor.FileManagement;
using Fotografix.IO;
using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    public sealed class ImportLayerCommand : IDocumentCommand
    {
        private readonly IImageDecoder imageDecoder;
        private readonly IFilePicker filePicker;

        public ImportLayerCommand(IImageDecoder imageDecoder, IFilePicker filePicker)
        {
            this.imageDecoder = imageDecoder;
            this.filePicker = filePicker;
        }

        public bool CanExecute(Document document)
        {
            return true;
        }

        public async Task ExecuteAsync(Document document)
        {
            var files = await filePicker.PickOpenFilesAsync(imageDecoder.SupportedFileFormats);

            foreach (var file in files)
            {
                Image importedImage = await imageDecoder.ReadImageAsync(file);
                foreach (Layer layer in importedImage.DetachLayers())
                {
                    document.Image.Layers.Add(layer);
                }
            }
        }
    }
}
