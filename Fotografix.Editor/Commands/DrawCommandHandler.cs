using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    public sealed class DrawCommandHandler : ICommandHandler<DrawCommand>
    {
        private readonly IGraphicsDevice graphicsDevice;

        public DrawCommandHandler(IGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public Task HandleAsync(DrawCommand command)
        {
            command.Channel.Draw(command.Drawable, graphicsDevice);
            return Task.CompletedTask;
        }
    }
}
