using Windows.Foundation;
using Windows.UI.Core;

namespace Fotografix.Input
{
    public sealed class NullPointerInputHandler : IPointerInputHandler
    {
        public CoreCursor Cursor { get; } = new CoreCursor(CoreCursorType.Arrow, 0);

        public bool PointerMoved(Point pt)
        {
            return false;
        }

        public bool PointerPressed(Point pt)
        {
            return false;
        }

        public bool PointerReleased(Point pt)
        {
            return false;
        }
    }
}
