namespace Fotografix.Editor.Commands
{
    /// <summary>
    /// Represents a change that can be merged into another change.
    /// </summary>
    /// <remarks>
    /// Mergeable changes are typically produced by commands that perform incremental
    /// changes, where successive applications of the same command can be merged into
    /// a single change.
    /// </remarks>
    public interface IMergeableChange : IChange
    {
        /// <summary>
        /// Attempts to merge the effects of this change into the specified change.
        /// </summary>
        /// <param name="change">The change to merge into.</param>
        /// <returns>true if this change was merged into the specified change; false otherwise.</returns>
        bool TryMergeInto(IChange change);
    }
}
