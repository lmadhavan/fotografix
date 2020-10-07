using Fotografix.Editor;
using Fotografix.UI;
using Fotografix.UI.FileManagement;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.UI
{
    public sealed class FakeCreateImageEditorCommand : ICreateImageEditorCommand
    {
        public string Title => "Test";

        public Task<ImageEditor> ExecuteAsync(Viewport viewport)
        {
            ImageEditor editor = new ImageEditor(new Image(Size.Empty), viewport);
            return Task.FromResult(editor);
        }
    }
}
