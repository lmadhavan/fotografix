using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    public sealed class NewLayerCommand : IDocumentCommand
    {
        public bool CanExecute(Document document)
        {
            return true;
        }

        public Task ExecuteAsync(Document document)
        {
            var layers = document.Image.Layers;
            string newLayerName = "Layer " + (layers.Count + 1);
            layers.Add(new Layer { Name = newLayerName });

            return Task.CompletedTask;
        }
    }
}
