namespace Fotografix.History
{
    public abstract class Change
    {
        public abstract void Undo();
        public abstract void Redo();

        public virtual bool TryMergeInto(Change change)
        {
            return false;
        }
    }
}
