using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.UI.FileManagement
{
    public sealed class NewImageCommand : ICreateImageEditorCommand
    {
        private readonly Size size;

        public NewImageCommand(Size size)
        {
            this.size = size;
        }

        public string Title => "Untitled";

        public Task<ImageEditor> ExecuteAsync()
        {
            return Task.FromResult(ImageEditor.Create(size));
        }
    }
}
