using Fotografix.Editor;

namespace Fotografix.Uwp
{
    public interface ICommandService
    {
        void Execute(Command command);
    }
}
