using Fotografix.Drawing;

namespace Fotografix.Editor.Drawing
{
    public sealed class DrawCommand : Command
    {
        private readonly Layer layer;
        private readonly IDrawingContextFactory drawingContextFactory;
        private readonly IDrawable drawable;
        private IUndoable undoable;

        public DrawCommand(Layer layer, IDrawingContextFactory drawingContextFactory, IDrawable drawable)
        {
            this.layer = layer;
            this.drawingContextFactory = drawingContextFactory;
            this.drawable = drawable;
        }

        public override void Execute()
        {
            this.undoable = layer.Draw(drawingContextFactory, drawable);
        }

        public override void Undo()
        {
            undoable.Undo();
        }
    }
}
