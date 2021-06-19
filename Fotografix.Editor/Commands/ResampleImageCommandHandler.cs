using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    public sealed class ResampleImageCommandHandler : ICommandHandler<ResampleImageCommand>
    {
        private readonly IGraphicsDevice graphicsDevice;

        public ResampleImageCommandHandler(IGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public Task HandleAsync(ResampleImageCommand command)
        {
            command.Image.Scale(command.NewSize, graphicsDevice);
            return Task.CompletedTask;
        }
    }
}
