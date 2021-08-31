using Windows.UI.Xaml.Controls;

namespace Fotografix
{
    public sealed partial class MainPage : Page
    {
        private readonly ApplicationViewModel vm;

        public MainPage()
        {
            this.InitializeComponent();
            this.vm = new ApplicationViewModel();
        }
    }
}
