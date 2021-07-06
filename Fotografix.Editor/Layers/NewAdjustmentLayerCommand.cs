using Fotografix.Adjustments;
using System.Threading.Tasks;

namespace Fotografix.Editor.Layers
{
    public sealed class NewAdjustmentLayerCommand<T> : IDocumentCommand where T : Adjustment, new()
    {
        private readonly string name;

        public NewAdjustmentLayerCommand(string name)
        {
            this.name = name;
        }

        public bool CanExecute(Document document)
        {
            return true;
        }

        public Task ExecuteAsync(Document document)
        {
            Layer layer = new Layer(new T()) { Name = name };
            document.Image.Layers.Add(layer);

            return Task.CompletedTask;
        }
    }
}
