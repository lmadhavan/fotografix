using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed record ReplaceItemChange<T>(IList<T> List, int Index, T OldItem, T NewItem) : IChange
    {
        public void Undo()
        {
            List[Index] = OldItem;
        }

        public void Redo()
        {
            List[Index] = NewItem;
        }
    }
}
