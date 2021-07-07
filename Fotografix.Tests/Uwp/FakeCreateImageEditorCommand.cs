using Fotografix.Editor;
using Fotografix.Uwp;
using Fotografix.Uwp.FileManagement;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Uwp
{
    public sealed class FakeCreateImageEditorCommand : ICreateImageEditorCommand
    {
        public Task<ImageEditor> ExecuteAsync(Viewport viewport)
        {
            ImageEditor editor = new ImageEditor(new Document(new Image(Size.Empty)));
            return Task.FromResult(editor);
        }
    }
}
