namespace Fotografix.Editor.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Prepares a change object representing the effects of the command.
        /// </summary>
        /// <returns>An <see cref="IChange"/> object representing the effects of the command; null if the command has no effect.</returns>
        IChange PrepareChange();
    }
}