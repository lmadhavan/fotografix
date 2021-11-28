using Windows.Foundation;
using Windows.UI.Core;

namespace Fotografix.Input
{
    public interface IPointerInputHandler
    {
        CoreCursor Cursor { get; }

        bool PointerPressed(Point pt);
        bool PointerMoved(Point pt);
        bool PointerReleased(Point pt);
    }
}
