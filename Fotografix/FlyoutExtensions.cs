using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Fotografix
{
    public static class FlyoutExtensions
    {
        public static void CloseParentFlyout(this FrameworkElement element)
        {
            while (element != null)
            {
                if (element is Popup popup)
                {
                    popup.IsOpen = false;
                    return;
                }

                element = element.Parent as FrameworkElement;
            }
        }
    }
}
