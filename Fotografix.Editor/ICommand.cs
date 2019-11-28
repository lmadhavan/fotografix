namespace Fotografix.Editor
{
    public interface ICommand
    {
        void Execute();
        void Undo();
        void Redo();
    }
}