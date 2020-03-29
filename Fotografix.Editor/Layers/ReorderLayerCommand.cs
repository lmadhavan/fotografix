namespace Fotografix.Editor.Layers
{
    public sealed class ReorderLayerCommand : Command
    {
        private readonly Image image;
        private readonly int oldIndex;
        private readonly int newIndex;

        public ReorderLayerCommand(Image image, int oldIndex, int newIndex)
        {
            this.image = image;
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
        }

        public override bool IsEffective => oldIndex != newIndex;

        public override void Execute()
        {
            image.Layers.Move(oldIndex, newIndex);
        }

        public override void Undo()
        {
            image.Layers.Move(newIndex, oldIndex);
        }
    }
}
