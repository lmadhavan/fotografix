using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Fotografix.UI
{
    public class TitleBar : ITitleBarLayoutMetrics
    {
        private readonly ApplicationViewTitleBar applicationViewTitleBar;
        private readonly CoreApplicationViewTitleBar coreTitleBar;
        private readonly Window window;

        private ICustomTitleBarProvider titleBarProvider;

        private TitleBar(ApplicationViewTitleBar applicationViewTitleBar, CoreApplicationViewTitleBar coreTitleBar, Window window)
        {
            this.applicationViewTitleBar = applicationViewTitleBar;
            this.coreTitleBar = coreTitleBar;
            this.window = window;
            
            coreTitleBar.LayoutMetricsChanged += OnLayoutMetricsChanged;
        }

        public static TitleBar GetForCurrentView()
        {
            return new TitleBar(ApplicationView.GetForCurrentView().TitleBar, CoreApplication.GetCurrentView().TitleBar, Window.Current);
        }

        public double SystemOverlayLeftInset => coreTitleBar.SystemOverlayLeftInset;
        public double SystemOverlayRightInset => coreTitleBar.SystemOverlayRightInset;
        public double Height => coreTitleBar.Height;

        public void Customize(ICustomTitleBarProvider titleBarProvider)
        {
            applicationViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            applicationViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            applicationViewTitleBar.ButtonForegroundColor = GetSystemColor("ChromeAltLow");
            applicationViewTitleBar.ButtonInactiveForegroundColor = GetSystemColor("ChromeDisabledLow");

            coreTitleBar.ExtendViewIntoTitleBar = true;

            window.SetTitleBar(titleBarProvider.CustomTitleBar);
            this.titleBarProvider = titleBarProvider;
        }

        private void OnLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            titleBarProvider?.UpdateLayout(this);
        }

        private static Color GetSystemColor(string name)
        {
            return (Color)Application.Current.Resources["System" + name + "Color"];
        }
    }
}
