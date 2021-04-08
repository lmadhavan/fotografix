using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed record AddItemChange<T>(IList<T> List, int Index, T Item) : IMergeableChange
    {
        public void Undo()
        {
            List.RemoveAt(Index);
        }

        public void Redo()
        {
            List.Insert(Index, Item);
        }

        public bool TryMergeWith(IChange previous, out IChange result)
        {
            if (previous is RemoveItemChange<T> remove &&
                remove.List == this.List &&
                EqualityComparer<T>.Default.Equals(remove.Item, this.Item))
            {
                result = new MoveItemChange<T>(List, remove.Index, this.Index);
                return true;
            }

            result = null;
            return false;
        }
    }
}
