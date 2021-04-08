using System;
using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed class ReplaceItemChange<T> : IChange, IEquatable<ReplaceItemChange<T>>
    {
        public ReplaceItemChange(IList<T> list, int index, T oldItem, T newItem)
        {
            this.List = list;
            this.Index = index;
            this.OldItem = oldItem;
            this.NewItem = newItem;
        }

        public IList<T> List { get; }
        public int Index { get; }
        public T OldItem { get; }
        public T NewItem { get; }

        public void Undo()
        {
            List[Index] = OldItem;
        }

        public void Redo()
        {
            List[Index] = NewItem;
        }

        public override string ToString()
        {
            return $"ReplaceItemChange [list={List}, index={Index}, oldItem={OldItem}, newItem={NewItem}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as ReplaceItemChange<T>);
        }

        public bool Equals(ReplaceItemChange<T> other)
        {
            return other != null &&
                   EqualityComparer<IList<T>>.Default.Equals(List, other.List) &&
                   Index == other.Index &&
                   EqualityComparer<T>.Default.Equals(OldItem, other.OldItem) &&
                   EqualityComparer<T>.Default.Equals(NewItem, other.NewItem);
        }

        public override int GetHashCode()
        {
            int hashCode = -1451364037;
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<T>>.Default.GetHashCode(List);
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(OldItem);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(NewItem);
            return hashCode;
        }

        public static bool operator ==(ReplaceItemChange<T> left, ReplaceItemChange<T> right)
        {
            return EqualityComparer<ReplaceItemChange<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(ReplaceItemChange<T> left, ReplaceItemChange<T> right)
        {
            return !(left == right);
        }

        #endregion
    }
}
