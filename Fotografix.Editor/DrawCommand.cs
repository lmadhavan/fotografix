namespace Fotografix.Editor
{
    public sealed class DrawCommand : Command
    {
        private readonly Layer layer;
        private readonly IDrawable drawable;
        private IUndoable undoable;

        public DrawCommand(Layer layer, IDrawable drawable)
        {
            this.layer = layer;
            this.drawable = drawable;
        }

        public override void Execute()
        {
            this.undoable = layer.Draw(drawable);
        }

        public override void Undo()
        {
            undoable.Undo();
        }
    }
}
