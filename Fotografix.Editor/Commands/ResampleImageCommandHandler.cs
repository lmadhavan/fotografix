namespace Fotografix.Editor.Commands
{
    public sealed class ResampleImageCommandHandler : ICommandHandler<ResampleImageCommand>
    {
        private readonly IBitmapResamplingStrategy resamplingStrategy;

        public ResampleImageCommandHandler(IBitmapResamplingStrategy resamplingStrategy)
        {
            this.resamplingStrategy = resamplingStrategy;
        }

        public void Handle(ResampleImageCommand command)
        {
            command.Image.Scale(command.NewSize, resamplingStrategy);
        }
    }
}
