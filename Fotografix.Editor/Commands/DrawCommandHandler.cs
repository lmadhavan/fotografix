namespace Fotografix.Editor.Commands
{
    public sealed class DrawCommandHandler : ICommandHandler<DrawCommand>
    {
        private readonly IGraphicsDevice graphicsDevice;

        public DrawCommandHandler(IGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Handle(DrawCommand command)
        {
            command.Channel.Draw(command.Drawable, graphicsDevice);
        }
    }
}
