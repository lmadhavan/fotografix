using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Fotografix
{
    public class CustomTitleBar
    {
        private readonly FrameworkElement titleBarElement;
        private readonly TextBlock titleElement;
        private readonly ApplicationViewTitleBar titleBar;
        private CoreWindowActivationState activationState;

        private CustomTitleBar(FrameworkElement titleBarElement, TextBlock titleElement)
        {
            this.titleBarElement = titleBarElement;
            this.titleElement = titleElement;
            this.titleBar = ApplicationView.GetForCurrentView().TitleBar;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            Window.Current.SetTitleBar(titleBarElement);
            Window.Current.Activated += Window_Activated;

            titleBarElement.ActualThemeChanged += TitleBarElement_ActualThemeChanged;
        }

        public static void Initialize(FrameworkElement titleBarElement, TextBlock titleElement)
        {
            var customTitleBar = new CustomTitleBar(titleBarElement, titleElement);
            customTitleBar.UpdateColors();
        }

        private void UpdateColors()
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];
            titleElement.Foreground = activationState == CoreWindowActivationState.Deactivated ? inactiveForegroundBrush : defaultForegroundBrush;

            titleBar.ButtonForegroundColor = (Color)Application.Current.Resources["SystemChromeAltLowColor"];
            titleBar.ButtonInactiveForegroundColor = (Color)Application.Current.Resources["SystemChromeDisabledLowColor"];

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            Thickness curMargin = titleBarElement.Margin;
            titleBarElement.Margin = new Thickness(sender.SystemOverlayLeftInset, curMargin.Top, sender.SystemOverlayRightInset, curMargin.Bottom);
        }

        private void TitleBarElement_ActualThemeChanged(FrameworkElement sender, object args)
        {
            UpdateColors();
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            this.activationState = e.WindowActivationState;
            UpdateColors();
        }
    }
}
