namespace Fotografix.Editor
{
    public abstract class Command
    {
        /// <summary>
        /// Gets a value indicating whether executing the command in its current state has an observable effect.
        /// </summary>
        public virtual bool IsEffective => true;

        /// <summary>
        /// Executes the command.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// Undoes the command.
        /// </summary>
        public abstract void Undo();

        /// <summary>
        /// Attempts to merge this command into the specified command.
        /// </summary>
        public virtual bool TryMergeInto(Command command)
        {
            return false;
        }
    }
}
