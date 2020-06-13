using System.Threading.Tasks;

namespace Fotografix.UI.FileManagement
{
    public interface IWorkspace
    {
        Task NewImageAsync();
        Task OpenFileAsync();
    }
}
