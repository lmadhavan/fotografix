namespace Fotografix
{
    public interface IMergeableChange : IChange
    {
        bool TryMergeWith(IChange previous, out IChange result);
    }
}
