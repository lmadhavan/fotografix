namespace Fotografix.Editor.Commands
{
    public interface ICommand
    {
        IChange PrepareChange();
    }
}