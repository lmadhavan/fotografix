using System;
using System.Collections.Generic;

namespace Fotografix.Collections
{
    public sealed class MoveItemChange<T> : IChange, IEquatable<MoveItemChange<T>>
    {
        public MoveItemChange(IList<T> list, int oldIndex, int newIndex)
        {
            this.List = list;
            this.OldIndex = oldIndex;
            this.NewIndex = newIndex;
        }

        public IList<T> List { get; }
        public int OldIndex { get; }
        public int NewIndex { get; }

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

        public override string ToString()
        {
            return $"MoveItemChange [list={List}, oldIndex={OldIndex}, newIndex={NewIndex}]";
        }

        #region Equals / GetHashCode

        public override bool Equals(object obj)
        {
            return Equals(obj as MoveItemChange<T>);
        }

        public bool Equals(MoveItemChange<T> other)
        {
            return other != null &&
                   EqualityComparer<IList<T>>.Default.Equals(List, other.List) &&
                   OldIndex == other.OldIndex &&
                   NewIndex == other.NewIndex;
        }

        public override int GetHashCode()
        {
            int hashCode = 1015106722;
            hashCode = hashCode * -1521134295 + EqualityComparer<IList<T>>.Default.GetHashCode(List);
            hashCode = hashCode * -1521134295 + OldIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + NewIndex.GetHashCode();
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
