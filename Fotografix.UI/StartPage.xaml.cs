using Fotografix.UI.FileManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Fotografix.UI
{
    public sealed partial class StartPage : Page
    {
        private IWorkspace workspace;

        public StartPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.workspace = (IWorkspace)e.Parameter;
        }

        private async void New_Click(object sender, RoutedEventArgs e)
        {
            await workspace.NewImageAsync();
        }

        private async void Open_Click(object sender, RoutedEventArgs e)
        {
            await workspace.OpenFileAsync();
        }
    }
}
