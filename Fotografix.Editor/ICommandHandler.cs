namespace Fotografix.Editor
{
    public interface ICommandHandler<T>
    {
        void Handle(T command);
    }
}
