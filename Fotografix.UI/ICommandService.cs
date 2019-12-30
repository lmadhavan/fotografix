using Fotografix.Editor.Commands;

namespace Fotografix.UI
{
    public interface ICommandService
    {
        void Execute(ICommand command);
    }
}
