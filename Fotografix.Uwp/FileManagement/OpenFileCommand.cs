using Fotografix.Editor;
using Fotografix.IO;
using System.Threading.Tasks;

namespace Fotografix.Uwp.FileManagement
{
    public sealed class OpenFileCommand : ICreateImageEditorCommand
    {
        private readonly IFile file;
        private readonly ImageEditorFactory imageEditorFactory;

        public OpenFileCommand(IFile file, ImageEditorFactory imageEditorFactory)
        {
            this.file = file;
            this.imageEditorFactory = imageEditorFactory;
        }

        public string Title => file.Name;

        public async Task<ImageEditor> ExecuteAsync(Viewport viewport)
        {
            return await imageEditorFactory.OpenImageAsync(viewport, file);
        }
    }
}