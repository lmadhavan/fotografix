using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class DrawCommand : IDocumentCommand
    {
        private readonly IGraphicsDevice graphicsDevice;

        public DrawCommand(IGraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public bool CanExecute(Document document)
        {
            var channel = document.ActiveChannel;
            return channel != null && channel.CanDraw && channel.GetDrawingPreview() != null;
        }

        public Task ExecuteAsync(Document document)
        {
            var channel = document.ActiveChannel;
            channel.Draw(channel.GetDrawingPreview(), graphicsDevice);
            return Task.CompletedTask;
        }
    }
}
