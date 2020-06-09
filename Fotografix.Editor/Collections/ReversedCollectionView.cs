using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Fotografix.Editor.Collections
{
    /// <summary>
    /// Provides a view of a list in which the order of elements is reversed.
    /// </summary>
    /// <remarks>
    /// The view is modifiable; any changes made through the view are propagated to the underlying collection.
    /// </remarks>
    public sealed class ReversedCollectionView<T> : IList<T>, IList, INotifyCollectionChanged, IDisposable
    {
        private readonly IList<T> list;

        public ReversedCollectionView(IList<T> list)
        {
            this.list = list;

            if (list is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)list).CollectionChanged += OnCollectionChanged;
            }
        }

        public void Dispose()
        {
            if (list is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)list).CollectionChanged -= OnCollectionChanged;
            }
        }

        public int Count => list.Count;

        public T this[int index]
        {
            get => list[Reverse(index)];
            set => list[Reverse(index)] = value;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        public int IndexOf(T item)
        {
            int index = list.IndexOf(item);
            return index == -1 ? -1 : Reverse(index);
        }

        public void Insert(int index, T item)
        {
            // Insert normally inserts before index, since the collection is reversed, we need to insert after index
            list.Insert(Reverse(index) + 1, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(Reverse(index));
        }

        public void Add(T item)
        {
            list.Insert(0, item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < Count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count == 1)
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, e.NewItems, Reverse(e.NewStartingIndex)));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems.Count == 1)
            {
                // Count has already decreased by the time this method is called, so we add 1 to get the correct reversed index
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, e.OldItems, Reverse(e.OldStartingIndex) + 1));
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace && e.OldItems.Count == 1)
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, e.NewItems, e.OldItems, Reverse(e.OldStartingIndex)));
            }
            else if (e.Action == NotifyCollectionChangedAction.Move && e.OldItems.Count == 1)
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, e.NewItems, Reverse(e.NewStartingIndex), Reverse(e.OldStartingIndex)));
            }
            else
            {
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        private int Reverse(int index)
        {
            return Count - index - 1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool ICollection.IsSynchronized => false;
        bool ICollection<T>.IsReadOnly => false;

        object ICollection.SyncRoot => ((IList)list).SyncRoot;

        bool IList.IsFixedSize => false;
        bool IList.IsReadOnly => false;

        object IList.this[int index] {
            get => this[index];
            set => this[index] = (T)value;
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }
    }
}
