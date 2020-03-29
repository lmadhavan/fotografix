namespace Fotografix.Editor.Layers
{
    public sealed class RemoveLayerCommand : Command
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

        public override void Execute()
        {
            image.Layers.RemoveAt(index);
        }

        public override void Undo()
        {
            image.Layers.Insert(index, layer);
        }
    }
}