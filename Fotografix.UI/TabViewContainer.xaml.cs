using Fotografix.UI.FileManagement;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
{
    public sealed partial class TabViewContainer : UserControl, ICustomTitleBarProvider, IWorkspace
    {
        public TabViewContainer()
        {
            this.InitializeComponent();
        }

        public async Task AddNewTabAsync()
        {
            StorageFile file = await GetSampleImageAsync();
            AddNewTab(new OpenFileCommand(file));
        }

        public async Task NewImageAsync()
        {
            NewImageParameters parameters = new NewImageParameters();

            NewImageDialog dialog = new NewImageDialog(parameters);
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                AddNewTab(new NewImageCommand(parameters.Size));
                ActivateLatestTab();
            }
        }

        public async Task OpenFileAsync()
        {
            FileOpenPicker picker = FilePickerFactory.CreateFilePicker();

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                AddNewTab(new OpenFileCommand(file));
                ActivateLatestTab();
            }
        }

        private void AddNewTab(ICreateImageEditorCommand command)
        {
            Frame frame = new Frame();
            frame.Navigate(typeof(ImageEditorPage), new ImageEditorPageParameters(workspace: this, command));

            TabViewItem tab = new TabViewItem()
            {
                Header = command.Title,
                Content = frame
            };
            tabView.TabItems.Add(tab);
        }

        private async void OnNewTabRequested(TabView sender, object args)
        {
            await AddNewTabAsync();
            ActivateLatestTab();
        }

        private void ActivateLatestTab()
        {
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
