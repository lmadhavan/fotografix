using System;
using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed class RemoveItemChange<T> : Change, IEquatable<RemoveItemChange<T>>
    {
        public RemoveItemChange(IList<T> list, int index, T item)
        {
            this.List = list;
            this.Index = index;
            this.Item = item;
        }

        public IList<T> List { get; }
        public int Index { get; }
        public T Item { get; }

        public override void Undo()
        {
            List.Insert(Index, Item);
        }

        public override void Redo()
        {
            List.RemoveAt(Index);
        }

        public override string ToString()
        {
            return $"RemoveItemChange [list={List}, index={Index}, item={Item}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as RemoveItemChange<T>);
        }

        public bool Equals(RemoveItemChange<T> other)
        {
            return other != null &&
                   EqualityComparer<IList<T>>.Default.Equals(List, other.List) &&
                   Index == other.Index &&
                   EqualityComparer<T>.Default.Equals(Item, other.Item);
        }

        public override int GetHashCode()
        {
            int hashCode = 1947568218;
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<T>>.Default.GetHashCode(List);
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Item);
            return hashCode;
        }

        public static bool operator ==(RemoveItemChange<T> left, RemoveItemChange<T> right)
        {
            return EqualityComparer<RemoveItemChange<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(RemoveItemChange<T> left, RemoveItemChange<T> right)
        {
            return !(left == right);
        }

        #endregion
    }
}
