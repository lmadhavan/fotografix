using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Fotografix.UI.Collections
{
    /// <summary>
    /// Provides a wrapper around a list that notifies clients when an item is reordered through
    /// a <see cref="Windows.UI.Xaml.Controls.ListView"/> control.
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Controls.ListViewBase.CanReorderItems"/>
    /// <remarks>
    /// The XAML ListView control implements item reordering by calling <see cref="IList{T}.RemoveAt(int)"/>
    /// and <see cref="IList{T}.Insert(int, T)"/> on the underlying list. This class detects this sequence
    /// of calls and raises an <see cref="ItemReordered"/> event.
    /// </remarks>
    public sealed class ReorderAwareCollectionView<T> : Collection<T>, INotifyCollectionChanged
    {
        private readonly IList<T> list;
        private int oldIndex;
        private T reorderItem;

        public ReorderAwareCollectionView(IList<T> list) : base(list)
        {
            this.list = list;
            this.oldIndex = -1;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => ((INotifyCollectionChanged)list).CollectionChanged += value;
            remove => ((INotifyCollectionChanged)list).CollectionChanged -= value;
        }

        /// <summary>
        /// Occurs when an item is reordered.
        /// </summary>
        public event ItemReorderedEventHandler ItemReordered;

        protected override void InsertItem(int index, T item)
        {
            if (oldIndex == -1 || !EqualityComparer<T>.Default.Equals(item, reorderItem))
            {
                throw new InvalidOperationException();
            }

            base.InsertItem(index, item);
            ItemReordered?.Invoke(this, new ItemReorderedEventArgs(oldIndex, index));

            this.oldIndex = -1;
            this.reorderItem = default;
        }

        protected override void RemoveItem(int index)
        {
            if (oldIndex != -1)
            {
                throw new InvalidOperationException();
            }

            this.oldIndex = index;
            this.reorderItem = this[index];

            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException();
        }

        protected override void ClearItems()
        {
            throw new NotSupportedException();
        }
    }

    public delegate void ItemReorderedEventHandler(object sender, ItemReorderedEventArgs args);

    public sealed class ItemReorderedEventArgs : EventArgs
    {
        public ItemReorderedEventArgs(int oldIndex, int newIndex)
        {
            this.OldIndex = oldIndex;
            this.NewIndex = newIndex;
        }

        public int OldIndex { get; }
        public int NewIndex { get; }
    }
}
