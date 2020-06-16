using Fotografix.UI.FileManagement;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Fotografix.UI
{
    public sealed partial class TabViewContainer : UserControl, ICustomTitleBarProvider, IWorkspace
    {
        public TabViewContainer()
        {
            this.InitializeComponent();
        }

        public void OpenStartPage()
        {
            CreateEmptyTab().OpenStartPage();
        }

        public async Task NewImageAsync()
        {
            NewImageParameters parameters = new NewImageParameters();

            NewImageDialog dialog = new NewImageDialog(parameters);
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                GetOrCreateEmptyTab().OpenImageEditor(new NewImageCommand(parameters.Size));
            }
        }

        public async Task OpenFileAsync()
        {
            FileOpenPicker picker = FilePickerFactory.CreateFilePicker();

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                GetOrCreateEmptyTab().OpenImageEditor(new OpenFileCommand(file));
            }
        }

        private Tab GetOrCreateEmptyTab()
        {
            Tab tab = (Tab)tabView.SelectedItem;

            if (tab != null && tab.IsEmpty)
            {
                return tab;
            }

            return CreateEmptyTab();
        }

        private Tab CreateEmptyTab()
        {
            Tab tab = new Tab(workspace: this);
            tabView.TabItems.Add(tab);

            if (IsLoaded)
            {
                tabView.SelectedItem = tab;
            }

            return tab;
        }

        #region TabView event handlers

        private void OnNewTabRequested(TabView sender, object args)
        {
            OpenStartPage();
        }

        private void OnCloseTabRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            Tab tab = (Tab)args.Tab;
            tabView.TabItems.Remove(tab);
            tab.Dispose();
        }

        #endregion

        #region Keyboard accelerator event handlers

        private async void OnNewImageInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            await NewImageAsync();
        }

        private async void OnOpenFileInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            await OpenFileAsync();
        }

        #endregion

        #region Title bar customization

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

        #endregion
    }
}
