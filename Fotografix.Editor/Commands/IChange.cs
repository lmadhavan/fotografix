namespace Fotografix.Editor.Commands
{
    /// <summary>
    /// Represents a reversible change.
    /// </summary>
    public interface IChange
    {
        /// <summary>
        /// Applies the change.
        /// </summary>
        void Apply();

        /// <summary>
        /// Undoes the change.
        /// </summary>
        void Undo();
    }
}
