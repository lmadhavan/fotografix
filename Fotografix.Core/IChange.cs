namespace Fotografix
{
    public interface IChange
    {
        void Undo();
        void Redo();
    }
}