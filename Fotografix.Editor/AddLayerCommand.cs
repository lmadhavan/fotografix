namespace Fotografix.Editor
{
    public sealed class AddLayerCommand : ICommand
    {
        private readonly Image image;
        private readonly Layer layer;

        public AddLayerCommand(Image image, Layer layer)
        {
            this.image = image;
            this.layer = layer;
        }

        public void Execute()
        {
            image.Layers.Add(layer);
        }

        public void Undo()
        {
            image.Layers.RemoveAt(image.Layers.Count - 1);
        }

        public void Redo()
        {
            Execute();
        }
    }
}
