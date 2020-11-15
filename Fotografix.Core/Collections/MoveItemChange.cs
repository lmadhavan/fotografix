using System;
using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed class MoveItemChange<T> : IChange, IEquatable<MoveItemChange<T>>
    {
        private readonly IList<T> list;
        private readonly int oldIndex;
        private readonly int newIndex;

        public MoveItemChange(IList<T> list, int oldIndex, int newIndex)
        {
            this.list = list;
            this.oldIndex = oldIndex;
            this.newIndex = newIndex;
        }

        public override string ToString()
        {
            return $"MoveItemChange [list={list}, oldIndex={oldIndex}, newIndex={newIndex}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as MoveItemChange<T>);
        }

        public bool Equals(MoveItemChange<T> other)
        {
            return other != null &&
                   EqualityComparer<IList<T>>.Default.Equals(list, other.list) &&
                   oldIndex == other.oldIndex &&
                   newIndex == other.newIndex;
        }

        public override int GetHashCode()
        {
            int hashCode = 1015106722;
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<T>>.Default.GetHashCode(list);
            hashCode = hashCode * -1521134295 + oldIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + newIndex.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(MoveItemChange<T> left, MoveItemChange<T> right)
        {
            return EqualityComparer<MoveItemChange<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(MoveItemChange<T> left, MoveItemChange<T> right)
        {
            return !(left == right);
        }

        #endregion
    }
}
