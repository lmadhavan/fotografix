using Windows.UI.Xaml;

namespace Fotografix.Uwp
{
    public interface ICustomTitleBarProvider
    {
        UIElement CustomTitleBar { get; }
        void UpdateLayout(ITitleBarLayoutMetrics titleBarLayoutMetrics);
    }
}
