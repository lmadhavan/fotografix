using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed record MoveItemChange<T>(IList<T> List, int OldIndex, int NewIndex) : IChange
    {
        public void Undo()
        {
            T item = List[NewIndex];
            List.RemoveAt(NewIndex);
            List.Insert(OldIndex, item);
        }

        public void Redo()
        {
            T item = List[OldIndex];
            List.RemoveAt(OldIndex);
            List.Insert(NewIndex, item);
        }
    }
}
