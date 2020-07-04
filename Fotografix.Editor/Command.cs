using Fotografix.History;

namespace Fotografix.Editor
{
    public abstract class Command : Change
    {
        /// <summary>
        /// Gets a value indicating whether executing the command in its current state has an observable effect.
        /// </summary>
        public virtual bool IsEffective => true;

        /// <summary>
        /// Executes the command.
        /// </summary>
        public abstract void Execute();

        public override void Redo()
        {
            Execute();
        }
    }
}
