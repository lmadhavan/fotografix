using Fotografix.Uwp;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Fotografix
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            WorkspaceView workspaceView = Window.Current.Content as WorkspaceView;

            if (workspaceView == null)
            {
                workspaceView = new WorkspaceView();
                TitleBar.GetForCurrentView().Customize(titleBarProvider: workspaceView);
                Window.Current.Content = workspaceView;
            }

            if (e.PrelaunchActivated == false)
            {
                workspaceView.OpenStartPage();
                Window.Current.Activate();
            }
        }

        private async void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await ReportExceptionAsync(e.Exception);
        }

        private async void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            await ReportExceptionAsync(e.Exception);
        }

        private async Task ReportExceptionAsync(Exception ex)
        {
            await new ErrorDialog(ex).ShowAsync();
        }
    }
}
