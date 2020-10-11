using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Core;

namespace Fotografix.Uwp.FileManagement
{
    public sealed class RecentFileList : IReadOnlyList<RecentFile>, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly StorageItemMostRecentlyUsedList mruList;

        public RecentFileList(StorageItemMostRecentlyUsedList mruList)
        {
            this.mruList = mruList;
            mruList.ItemRemoved += OnItemRemoved;
        }

        public int Count => mruList.Entries.Count;
        public RecentFile this[int index] => new RecentFile(mruList.Entries[index]);
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerator<RecentFile> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        public void Add(StorageFile file)
        {
            mruList.Add(file, file.DisplayName);
            RaiseCollectionReset();
        }

        public void Clear()
        {
            mruList.Clear();
        }

        private void RaiseCollectionReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        }

        private async void OnItemRemoved(StorageItemMostRecentlyUsedList sender, ItemRemovedEventArgs args)
        {
            // ItemRemoved is raised on a non-UI thread so the collection event must be dispatched back to the UI thread
            await CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, RaiseCollectionReset);
        }

        public Task<StorageFile> GetFileAsync(RecentFile recentFile)
        {
            return mruList.GetFileAsync(recentFile.Token).AsTask();
        }

        #region IList implementation

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => true;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        object IList.this[int index]
        {
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
            return false;
        }

        int IList.IndexOf(object value)
        {
            return -1;
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
            throw new NotSupportedException();
        }

        #endregion
    }
}