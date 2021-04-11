namespace Fotografix.Editor.Commands
{
    public interface ICommandDispatcher
    {
        void Dispatch<T>(T command);
    }
}
