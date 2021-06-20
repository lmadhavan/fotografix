using Fotografix.Editor;
using Fotografix.Editor.ChangeTracking;
using Fotografix.Editor.Commands;
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
            ImageEditor editor = new ImageEditor(new Image(Size.Empty), new History(), new CommandHandlerCollection());
            return Task.FromResult(editor);
        }
    }
}
