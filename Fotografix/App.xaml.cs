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

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            TabViewContainer container = Window.Current.Content as TabViewContainer;

            if (container == null)
            {
                container = new TabViewContainer();
                TitleBar.GetForCurrentView().Customize(titleBarProvider: container);
                Window.Current.Content = container;
            }

            if (e.PrelaunchActivated == false)
            {
                container.OpenStartPage();
                Window.Current.Activate();
            }
        }
    }
}
