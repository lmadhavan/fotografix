using System;
using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed class ReplaceItemChange<T> : IChange, IEquatable<ReplaceItemChange<T>>
    {
        private readonly IList<T> list;
        private readonly int index;
        private readonly T oldItem;
        private readonly T newItem;

        public ReplaceItemChange(IList<T> list, int index, T oldItem, T newItem)
        {
            this.list = list;
            this.index = index;
            this.oldItem = oldItem;
            this.newItem = newItem;
        }

        public override string ToString()
        {
            return $"ReplaceItemChange [list={list}, index={index}, oldItem={oldItem}, newItem={newItem}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as ReplaceItemChange<T>);
        }

        public bool Equals(ReplaceItemChange<T> other)
        {
            return other != null &&
                   EqualityComparer<IList<T>>.Default.Equals(list, other.list) &&
                   index == other.index &&
                   EqualityComparer<T>.Default.Equals(oldItem, other.oldItem) &&
                   EqualityComparer<T>.Default.Equals(newItem, other.newItem);
        }

        public override int GetHashCode()
        {
            int hashCode = -1451364037;
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<T>>.Default.GetHashCode(list);
            hashCode = hashCode * -1521134295 + index.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(oldItem);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(newItem);
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
