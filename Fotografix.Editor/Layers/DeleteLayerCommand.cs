using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    public sealed class DeleteLayerCommand : IDocumentCommand
    {
        public bool CanExecute(Document document)
        {
            return document.Image.Layers.Count > 1;
        }

        public Task ExecuteAsync(Document document)
        {
            Image image = document.Image;
            image.Layers.Remove(document.ActiveLayer);

            return Task.CompletedTask;
        }
    }
}
