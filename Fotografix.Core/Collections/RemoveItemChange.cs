using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed record RemoveItemChange<T>(IList<T> List, int Index, T Item) : IChange
    {
        public void Undo()
        {
            List.Insert(Index, Item);
        }

        public void Redo()
        {
            List.RemoveAt(Index);
        }
    }
}
