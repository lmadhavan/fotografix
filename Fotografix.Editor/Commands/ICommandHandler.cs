namespace Fotografix.Editor.Commands
{
    public interface ICommandHandler<T>
    {
        void Handle(T command);
    }
}
