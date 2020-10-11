using Fotografix.Editor;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Uwp.FileManagement
{
    public sealed class NewImageCommand : ICreateImageEditorCommand
    {
        private readonly Size size;
        private readonly ImageEditorFactory imageEditorFactory;

        public NewImageCommand(Size size, ImageEditorFactory imageEditorFactory)
        {
            this.size = size;
            this.imageEditorFactory = imageEditorFactory;
        }

        public string Title => "Untitled";

        public Task<ImageEditor> ExecuteAsync(Viewport viewport)
        {
            return Task.FromResult(imageEditorFactory.CreateNewImage(viewport, size));
        }
    }
}
