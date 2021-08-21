using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Fotografix.Uwp
{
    public class TitleBar : ITitleBarLayoutMetrics
    {
        private readonly ApplicationViewTitleBar applicationViewTitleBar;
        private readonly CoreApplicationViewTitleBar coreTitleBar;
        private readonly Window window;
        private readonly UISettings uiSettings;

        private ICustomTitleBarProvider titleBarProvider;

        private TitleBar(ApplicationViewTitleBar applicationViewTitleBar, CoreApplicationViewTitleBar coreTitleBar, Window window)
        {
            this.applicationViewTitleBar = applicationViewTitleBar;
            this.coreTitleBar = coreTitleBar;
            this.window = window;
            this.uiSettings = new UISettings();
            
            coreTitleBar.LayoutMetricsChanged += OnLayoutMetricsChanged;
            uiSettings.ColorValuesChanged += OnColorValuesChanged;
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
            UpdateTitleBarColors();

            coreTitleBar.ExtendViewIntoTitleBar = true;

            window.SetTitleBar(titleBarProvider.CustomTitleBar);
            this.titleBarProvider = titleBarProvider;
        }

        private void UpdateTitleBarColors()
        {
            applicationViewTitleBar.ButtonBackgroundColor = Colors.Transparent;
            applicationViewTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            applicationViewTitleBar.ButtonForegroundColor = GetSystemColor("ChromeAltLow");
            applicationViewTitleBar.ButtonInactiveForegroundColor = GetSystemColor("ChromeDisabledLow");
        }

        private void OnLayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            titleBarProvider?.UpdateLayout(this);
        }

        private async void OnColorValuesChanged(UISettings sender, object args)
        {
            await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, UpdateTitleBarColors);
        }

        private static Color GetSystemColor(string name)
        {
            return (Color)Application.Current.Resources["System" + name + "Color"];
        }
    }
}
