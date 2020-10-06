using Fotografix.Editor;
using Fotografix.IO;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.UI.FileManagement
{
    public sealed class NewImageCommand : ICreateImageEditorCommand
    {
        private readonly Size size;
        private readonly IImageDecoder imageDecoder;

        public NewImageCommand(Size size, IImageDecoder imageDecoder)
        {
            this.size = size;
            this.imageDecoder = imageDecoder;
        }

        public string Title => "Untitled";

        public Task<ImageEditor> ExecuteAsync(Viewport viewport)
        {
            return Task.FromResult(ImageEditor.Create(size, viewport, imageDecoder));
        }
    }
}
