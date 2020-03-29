using Fotografix.Editor;

namespace Fotografix.UI
{
    public interface ICommandService
    {
        void Execute(Command command);
    }
}
