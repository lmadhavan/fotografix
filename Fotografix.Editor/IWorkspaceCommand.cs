using System.Threading.Tasks;

namespace Fotografix.Editor
{
    public interface IWorkspaceCommand
    {
        Task ExecuteAsync(Workspace workspace);
    }
}
