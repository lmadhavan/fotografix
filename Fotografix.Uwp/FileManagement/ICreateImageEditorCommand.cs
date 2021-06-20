using Fotografix.Editor;
using System.Threading.Tasks;

namespace Fotografix.Uwp.FileManagement
{
    public interface ICreateImageEditorCommand
    {
        Task<ImageEditor> ExecuteAsync(Viewport viewport);
    }
}
