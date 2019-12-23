namespace Fotografix.Editor.Commands
{
    public sealed class ReorderLayerCommand : ICommand, IChange
    {
        private readonly Image image;

        public ReorderLayerCommand(Image image, int fromIndex, int toIndex)
        {
            this.image = image;
            this.FromIndex = fromIndex;
            this.ToIndex = toIndex;
        }

        public int FromIndex { get; }
        public int ToIndex { get; }

        public IChange PrepareChange()
        {
            return this;
        }

        void IChange.Apply()
        {
            image.Layers.Move(FromIndex, ToIndex);
        }

        void IChange.Undo()
        {
            image.Layers.Move(ToIndex, FromIndex);
        }
    }
}
