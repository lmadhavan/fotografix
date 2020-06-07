using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
{
    public sealed partial class TabViewContainer : UserControl, ICustomTitleBarProvider
    {
        public TabViewContainer()
        {
            this.InitializeComponent();
        }

        public async Task AddNewTab()
        {
            StorageFile file = await GetSampleImageAsync();

            Frame frame = new Frame();
            frame.Navigate(typeof(ImageEditorPage), file);

            TabViewItem tab = new TabViewItem()
            {
                Header = file.DisplayName,
                Content = frame
            };
            tabView.TabItems.Add(tab);
        }

        private async void OnAddTabRequested(TabView sender, object args)
        {
            await AddNewTab();
            tabView.SelectedItem = tabView.TabItems.Last();
        }

        private void OnCloseTabRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            ImageEditorPage page = GetPage(args.Tab);
            tabView.TabItems.Remove(args.Tab);
            page.Dispose();
        }

        private async Task<StorageFile> GetSampleImageAsync()
        {
            var samplesFolder = await Package.Current.InstalledLocation.GetFolderAsync("Sample Images");
            return await samplesFolder.GetFileAsync("flowers.jpg");
        }

        private ImageEditorPage GetPage(TabViewItem tab)
        {
            Frame frame = (Frame)tab.Content;
            return (ImageEditorPage)frame.Content;
        }

        UIElement ICustomTitleBarProvider.CustomTitleBar => tabStripFooter;

        void ICustomTitleBarProvider.UpdateLayout(ITitleBarLayoutMetrics metrics)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                tabStripHeader.MinWidth = metrics.SystemOverlayLeftInset;
                tabStripFooter.MinWidth = metrics.SystemOverlayRightInset;
            }
            else
            {
                tabStripHeader.MinWidth = metrics.SystemOverlayRightInset;
                tabStripFooter.MinWidth = metrics.SystemOverlayLeftInset;
            }

            tabStripHeader.Height = tabStripFooter.Height = metrics.Height;
        }
    }
}
