using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class ResizeImageCommand : IDocumentCommand
    {
        private readonly IDialog<ResizeImageParameters> resizeImageDialog;
        private readonly IGraphicsDevice graphicsDevice;

        public ResizeImageCommand(IDialog<ResizeImageParameters> resizeImageDialog, IGraphicsDevice graphicsDevice)
        {
            this.resizeImageDialog = resizeImageDialog;
            this.graphicsDevice = graphicsDevice;
        }

        public bool CanExecute(Document document)
        {
            return true;
        }

        public async Task ExecuteAsync(Document document)
        {
            ResizeImageParameters parameters = new(document.Image.Size);

            if (await resizeImageDialog.ShowAsync(parameters))
            {
                document.Image.Scale(parameters.Size, graphicsDevice);
            }
        }
    }
}
