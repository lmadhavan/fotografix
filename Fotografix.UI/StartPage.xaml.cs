using Fotografix.UI.FileManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.UI
{
    public sealed partial class StartPage : Page
    {
        private Workspace workspace;

        public StartPage()
        {
            this.InitializeComponent();
        }

        private RecentFileList RecentFiles => workspace.RecentFiles;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.workspace = (Workspace)e.Parameter;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            newButton.Focus(FocusState.Programmatic);
        }

        private Visibility VisibleWhenZero(int count)
        {
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void New_Click(object sender, RoutedEventArgs e)
        {
            await workspace.NewImageAsync();
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            await workspace.OpenFileAsync();
        }

        private async void RecentFileList_ItemClick(object sender, ItemClickEventArgs e)
        {
            await workspace.OpenRecentFileAsync((RecentFile)e.ClickedItem);
        }
    }
}
