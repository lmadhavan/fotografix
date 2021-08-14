using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public sealed class ResizeImageCommand : DocumentCommand
    {
        private readonly IDialog<ResizeImageParameters> resizeImageDialog;
        private readonly IGraphicsDevice graphicsDevice;

        public ResizeImageCommand(IDialog<ResizeImageParameters> resizeImageDialog, IGraphicsDevice graphicsDevice)
        {
            this.resizeImageDialog = resizeImageDialog;
            this.graphicsDevice = graphicsDevice;
        }

        public async override Task ExecuteAsync(Document document, object parameter, CancellationToken cancellationToken, IProgress<EditorCommandProgress> progress)
        {
            ResizeImageParameters parameters = new(document.Image.Size);

            if (await resizeImageDialog.ShowAsync(parameters))
            {
                document.Image.Scale(parameters.Size, graphicsDevice);
            }
        }
    }
}
