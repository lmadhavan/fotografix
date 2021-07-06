using Fotografix.Uwp.FileManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.Uwp
{
    public sealed partial class StartPage : Page
    {
        private FileManager fileManager;

        public StartPage()
        {
            this.InitializeComponent();
        }

        private RecentFileList RecentFiles => RecentFileList.Default;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.fileManager = (FileManager)e.Parameter;
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
            await fileManager.NewImageAsync();
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            await fileManager.OpenFileAsync();
        }

        private async void RecentFileList_ItemClick(object sender, ItemClickEventArgs e)
        {
            await fileManager.OpenRecentFileAsync((RecentFile)e.ClickedItem);
        }
    }
}
