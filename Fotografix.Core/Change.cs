namespace Fotografix
{
    public abstract class Change
    {
        public abstract void Undo();
        public abstract void Redo();

        public virtual bool TryMergeWith(Change previous, out Change result)
        {
            result = null;
            return false;
        }
    }
}