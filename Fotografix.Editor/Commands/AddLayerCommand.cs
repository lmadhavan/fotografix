namespace Fotografix.Editor.Commands
{
    public sealed class AddLayerCommand : ICommand, IChange
    {
        private readonly Image image;
        private readonly Layer layer;

        public AddLayerCommand(Image image, Layer layer)
        {
            this.image = image;
            this.layer = layer;
        }

        public IChange PrepareChange()
        {
            return this;
        }

        void IChange.Apply()
        {
            image.Layers.Add(layer);
        }

        void IChange.Undo()
        {
            image.Layers.RemoveAt(image.Layers.Count - 1);
        }
    }
}
