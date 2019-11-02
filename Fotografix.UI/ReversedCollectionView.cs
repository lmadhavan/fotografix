using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Fotografix.UI
{
    public sealed class ReversedCollectionView<T> : IReadOnlyList<T>, IList, INotifyCollectionChanged, IDisposable
    {
        private ReadOnlyObservableCollection<T> collection;

        public ReversedCollectionView(ReadOnlyObservableCollection<T> collection)
        {
            this.collection = collection;
            ((INotifyCollectionChanged)collection).CollectionChanged += OnCollectionChanged;
        }

        public void Dispose()
        {
            ((INotifyCollectionChanged)collection).CollectionChanged -= OnCollectionChanged;
        }

        public int Count => collection.Count;

        public T this[int index] => collection[Reverse(index)];

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
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

        int ICollection.Count => Count;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => ((IList)collection).SyncRoot;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => true;

        object IList.this[int index] {
            get => this[index];
            set => throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object value)
        {
            return ((IList)collection).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return Reverse(((IList)collection).IndexOf(value));
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)collection).CopyTo(array, index);
        }
    }
}
