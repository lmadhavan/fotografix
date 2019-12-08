namespace Fotografix.Editor.Commands
{
    public sealed class RemoveLayerCommand : ICommand, IChange
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

        public IChange PrepareChange()
        {
            return this;
        }

        void IChange.Apply()
        {
            image.Layers.RemoveAt(index);
        }

        void IChange.Undo()
        {
            image.Layers.Insert(index, layer);
        }
    }
}