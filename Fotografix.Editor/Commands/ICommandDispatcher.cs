using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    public interface ICommandDispatcher
    {
        Task DispatchAsync<T>(T command);
    }
}
