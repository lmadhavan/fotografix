namespace Fotografix.Editor.Layers
{
    public sealed class AddLayerCommand : Command
    {
        private readonly Image image;
        private readonly Layer layer;

        public AddLayerCommand(Image image, Layer layer)
        {
            this.image = image;
            this.layer = layer;
        }

        public override void Execute()
        {
            image.Layers.Add(layer);
        }

        public override void Undo()
        {
            image.Layers.RemoveAt(image.Layers.Count - 1);
        }
    }
}
