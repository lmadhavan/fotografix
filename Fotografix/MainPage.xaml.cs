using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix
{
    public sealed partial class MainPage : Page
    {
        private ApplicationViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            CustomTitleBar.Initialize(appTitleBar, appTitle);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.vm = (ApplicationViewModel)e.Parameter;
            vm.EditorInvalidated += (s, e2) => canvas.Invalidate();
        }

        private async void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            await vm.DisposeAsync();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            vm.Editor?.Result?.Draw(args.DrawingSession);
        }

        private void RecentFolderFlyout_FolderActivated(object sender, StorageFolder folder)
        {
            vm.OpenFolder(folder);
        }

        private async void PickFolder()
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add("*");

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                recentFolderFlyout.Add(folder);
                vm.OpenFolder(folder);
            }
        }

        private string FormatPhotoCount(int count)
        {
            string suffix = count == 1 ? "" : "s";
            return $"({count} photo{suffix})";
        }
    }
}
