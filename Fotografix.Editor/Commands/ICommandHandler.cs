using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    public interface ICommandHandler<T>
    {
        Task HandleAsync(T command);
    }
}
