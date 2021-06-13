using Fotografix.Drawing;

namespace Fotografix.Editor.Commands
{
    public sealed class ResampleImageCommandHandler : ICommandHandler<ResampleImageCommand>
    {
        private readonly IDrawingContextFactory drawingContextFactory;

        public ResampleImageCommandHandler(IDrawingContextFactory drawingContextFactory)
        {
            this.drawingContextFactory = drawingContextFactory;
        }

        public void Handle(ResampleImageCommand command)
        {
            command.Image.Scale(command.NewSize, drawingContextFactory);
        }
    }
}
