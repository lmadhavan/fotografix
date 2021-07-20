using Fotografix.Editor;
using System;
using System.Collections.Specialized;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp
{
    public static class ListViewWorkaround
    {
        /// <summary>
        /// There is an issue with ListView where the selection does not update if SelectedItem
        /// is set before a CollectionChanged event is received. To work around this, we need to
        /// reset the selection AFTER the ListView has processed CollectionChanged.
        /// </summary>
        public static IDisposable BindSelectedItem(this ListView listView, Func<object> selectedItemProvider, INotifyCollectionChanged sourceCollection)
        {
            void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                listView.SelectedItem = null;
                listView.SelectedItem = selectedItemProvider();
            }

            sourceCollection.CollectionChanged += OnCollectionChanged;
            return new DisposableAction(() => sourceCollection.CollectionChanged -= OnCollectionChanged);
        }
    }
}
