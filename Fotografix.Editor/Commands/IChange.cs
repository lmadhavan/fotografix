namespace Fotografix.Editor.Commands
{
    public interface IChange
    {
        void Apply();
        void Undo();
    }
}
