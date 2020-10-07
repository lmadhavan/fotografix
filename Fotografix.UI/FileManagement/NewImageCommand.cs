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
        private readonly IImageEncoder imageEncoder;

        public NewImageCommand(Size size, IImageDecoder imageDecoder, IImageEncoder imageEncoder)
        {
            this.size = size;
            this.imageDecoder = imageDecoder;
            this.imageEncoder = imageEncoder;
        }

        public string Title => "Untitled";

        public Task<ImageEditor> ExecuteAsync(Viewport viewport)
        {
            return Task.FromResult(ImageEditor.Create(size, viewport, imageDecoder, imageEncoder));
        }
    }
}
