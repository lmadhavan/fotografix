namespace Fotografix.Editor
{
    public sealed class PaintBrushStrokeCommand : Command
    {
        private readonly Layer layer;
        private readonly BrushStroke brushStroke;
        private IUndoable undoable;

        public PaintBrushStrokeCommand(Layer layer, BrushStroke brushStroke)
        {
            this.layer = layer;
            this.brushStroke = brushStroke;
        }

        public override void Execute()
        {
            this.undoable = layer.Paint(brushStroke);
        }

        public override void Undo()
        {
            undoable.Undo();
        }
    }
}
