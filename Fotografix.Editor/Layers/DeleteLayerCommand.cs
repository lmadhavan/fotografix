namespace Fotografix.Editor.Layers
{
    public sealed class DeleteLayerCommand : SynchronousDocumentCommand
    {
        public override bool CanExecute(Document document, object parameter)
        {
            return document.Image.Layers.Count > 1;
        }

        public override void Execute(Document document, object parameter)
        {
            Image image = document.Image;
            image.Layers.Remove(document.ActiveLayer);
        }
    }
}
