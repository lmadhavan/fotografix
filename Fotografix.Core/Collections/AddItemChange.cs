using System;
using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed class AddItemChange<T> : Change, IEquatable<AddItemChange<T>>
    {
        public AddItemChange(IList<T> list, int index, T item)
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
            List.RemoveAt(Index);
        }

        public override void Redo()
        {
            List.Insert(Index, Item);
        }

        public override bool TryMergeWith(Change previous, out Change result)
        {
            if (previous is RemoveItemChange<T> remove &&
                remove.List == this.List &&
                EqualityComparer<T>.Default.Equals(remove.Item, this.Item))
            {
                result = new MoveItemChange<T>(List, remove.Index, this.Index);
                return true;
            }

            return base.TryMergeWith(previous, out result);
        }

        public override string ToString()
        {
            return $"AddItemChange [list={List}, index={Index}, item={Item}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as AddItemChange<T>);
        }

        public bool Equals(AddItemChange<T> other)
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

        public static bool operator ==(AddItemChange<T> left, AddItemChange<T> right)
        {
            return EqualityComparer<AddItemChange<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(AddItemChange<T> left, AddItemChange<T> right)
        {
            return !(left == right);
        }

        #endregion
    }
}
