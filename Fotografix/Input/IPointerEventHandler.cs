using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace Fotografix.Input
{
    public interface IPointerEventHandler
    {
        CoreCursor Cursor { get; }

        bool PointerPressed(PointerRoutedEventArgs e);
        bool PointerMoved(PointerRoutedEventArgs e);
        bool PointerReleased(PointerRoutedEventArgs e);
    }
}
