using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class CropCommand : IDocumentCommand
    {
        public bool CanExecute(Document document)
        {
            return document.Image.GetCropPreview() != null;
        }

        public Task ExecuteAsync(Document document)
        {
            var image = document.Image;
            image.Crop(image.GetCropPreview().Value);
            return Task.CompletedTask;
        }
    }
}
