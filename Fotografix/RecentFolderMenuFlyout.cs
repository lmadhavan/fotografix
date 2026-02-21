using System;
using System.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix
{
    public sealed class RecentFolderMenuFlyout : MenuFlyout
    {
        private readonly StorageItemMostRecentlyUsedList mruList;

        public RecentFolderMenuFlyout()
        {
            this.mruList = StorageApplicationPermissions.MostRecentlyUsedList;
            this.Opening += OnOpening;
        }

        public int MaxItems { get; set; } = 5;
        public event EventHandler<StorageFolder> FolderActivated;

        public void Add(StorageFolder folder)
        {
            mruList.Add(folder, folder.DisplayName);
        }

        public void Clear()
        {
            mruList.Clear();
        }

        private void OnOpening(object sender, object e)
        {
            var lastSeparator = Items.OfType<MenuFlyoutSeparator>().Last();

            for (int i = Items.Count - 1; i > Items.IndexOf(lastSeparator); i--)
            {
                Items.RemoveAt(i);
            }

            if (mruList.Entries.Count == 0)
            {
                Items.Add(new MenuFlyoutItem { Text = "No recent folders", IsEnabled = false });
                return;
            }

            foreach (var entry in mruList.Entries.Take(MaxItems))
            {
                var item = new MenuFlyoutItem { Text = entry.Metadata, Tag = entry.Token };
                item.Click += OnRecentFolderClicked;
                Items.Add(item);
            }
        }

        private async void OnRecentFolderClicked(object sender, RoutedEventArgs e)
        {
            string token = (string)((MenuFlyoutItem)sender).Tag;

            try
            {
                var folder = await mruList.GetFolderAsync(token);
                Logger.LogEvent("OpenRecentFolder");
                FolderActivated?.Invoke(this, folder);
            }
            catch (Exception)
            {
                mruList.Remove(token);

                var dialog = new ContentDialog
                {
                    Title = "Unable to open folder",
                    Content = "The selected folder is no longer available.",
                    CloseButtonText = "OK"
                };

                await dialog.ShowAsync();
            }
        }
    }
}
