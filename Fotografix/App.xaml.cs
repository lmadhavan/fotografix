using Fotografix.UI;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Fotografix
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            TabViewContainer container = Window.Current.Content as TabViewContainer;

            if (container == null)
            {
                container = new TabViewContainer();
                Window.Current.Content = container;
            }

            if (e.PrelaunchActivated == false)
            {
                await container.AddNewTab();
                Window.Current.Activate();
            }
        }
    }
}
