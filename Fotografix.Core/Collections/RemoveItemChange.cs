using System;
using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed class RemoveItemChange<T> : IChange, IEquatable<RemoveItemChange<T>>
    {
        private readonly IList<T> list;
        private readonly int index;
        private readonly T item;

        public RemoveItemChange(IList<T> list, int index, T item)
        {
            this.list = list;
            this.index = index;
            this.item = item;
        }

        public override string ToString()
        {
            return $"RemoveItemChange [list={list}, index={index}, item={item}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as RemoveItemChange<T>);
        }

        public bool Equals(RemoveItemChange<T> other)
        {
            return other != null &&
                   EqualityComparer<IList<T>>.Default.Equals(list, other.list) &&
                   index == other.index &&
                   EqualityComparer<T>.Default.Equals(item, other.item);
        }

        public override int GetHashCode()
        {
            int hashCode = 1947568218;
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<T>>.Default.GetHashCode(list);
            hashCode = hashCode * -1521134295 + index.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(item);
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
