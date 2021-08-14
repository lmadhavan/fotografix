namespace Fotografix.Editor.Layers
{
    public sealed class NewLayerCommand : SynchronousDocumentCommand
    {
        public override void Execute(Document document, object parameter)
        {
            var layers = document.Image.Layers;
            string newLayerName = "Layer " + (layers.Count + 1);
            layers.Add(new Layer { Name = newLayerName });
        }
    }
}
