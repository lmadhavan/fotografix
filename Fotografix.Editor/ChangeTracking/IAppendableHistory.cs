namespace Fotografix.Editor.ChangeTracking
{
    public interface IAppendableHistory : IHistory
    {
        void Add(IChange change);
    }
}
