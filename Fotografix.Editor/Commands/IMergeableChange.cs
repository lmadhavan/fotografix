namespace Fotografix.Editor.Commands
{
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
