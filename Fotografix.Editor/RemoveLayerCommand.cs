namespace Fotografix.Editor
{
    public sealed class RemoveLayerCommand : ICommand
    {
        private readonly Image image;
        private readonly Layer layer;
        private readonly int index;

        public RemoveLayerCommand(Image image, Layer layer)
        {
            this.image = image;
            this.layer = layer;
            this.index = image.Layers.IndexOf(layer);
        }

        public void Execute()
        {
            image.Layers.RemoveAt(index);
        }

        public void Undo()
        {
            image.Layers.Insert(index, layer);
        }

        public void Redo()
        {
            Execute();
        }
    }
}