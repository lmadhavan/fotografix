namespace Fotografix.Editor.Commands
{
    public sealed class ReorderLayerCommand : ICommand, IChange
    {
        private readonly Image image;

        public ReorderLayerCommand(Image image, int oldIndex, int newIndex)
        {
            this.image = image;
            this.OldIndex = oldIndex;
            this.NewIndex = newIndex;
        }

        public int OldIndex { get; }
        public int NewIndex { get; }

        public IChange PrepareChange()
        {
            return this;
        }

        void IChange.Apply()
        {
            image.Layers.Move(OldIndex, NewIndex);
        }

        void IChange.Undo()
        {
            image.Layers.Move(NewIndex, OldIndex);
        }
    }
}
