using Fotografix.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed class DrawCommandHandler : ICommandHandler<DrawCommand>
    {
        private readonly IDrawingContextFactory drawingContextFactory;

        public DrawCommandHandler(IDrawingContextFactory drawingContextFactory)
        {
            this.drawingContextFactory = drawingContextFactory;
        }

        public void Handle(DrawCommand command)
        {
            command.Layer.Draw(command.Drawable, drawingContextFactory);
        }
    }
}
