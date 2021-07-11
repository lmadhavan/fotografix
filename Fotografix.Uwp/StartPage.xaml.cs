using Fotografix.Uwp.FileManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.Uwp
{
    public sealed partial class StartPage : Page
    {
        private IStartPageViewModel vm;

        public StartPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.vm = (IStartPageViewModel)e.Parameter;
            Bindings.Update();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            newButton.Focus(FocusState.Programmatic);
        }

        private Visibility VisibleWhenZero(int count)
        {
            return count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void RecentFileList_ItemClick(object sender, ItemClickEventArgs e)
        {
            await vm.OpenRecentFileAsync((RecentFile)e.ClickedItem);
        }
    }
}
