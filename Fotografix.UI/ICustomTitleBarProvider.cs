using Windows.UI.Xaml;

namespace Fotografix.UI
{
    public interface ICustomTitleBarProvider
    {
        UIElement CustomTitleBar { get; }
        void UpdateLayout(ITitleBarLayoutMetrics titleBarLayoutMetrics);
    }
}
