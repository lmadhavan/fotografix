using Fotografix.Editor;
using System.Threading.Tasks;

namespace Fotografix.UI.FileManagement
{
    public interface ICreateImageEditorCommand
    {
        string Title { get; }
        Task<ImageEditor> ExecuteAsync(Viewport viewport);
    }
}
