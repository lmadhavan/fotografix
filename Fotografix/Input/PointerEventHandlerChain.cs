using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace Fotografix.Input
{
    public sealed class PointerEventHandlerChain : IPointerEventHandler
    {
        private readonly IEnumerable<IPointerEventHandler> handlers;

        public PointerEventHandlerChain(IEnumerable<IPointerEventHandler> handlers)
        {
            this.handlers = handlers;
        }

        public CoreCursor Cursor { get; private set; } = new CoreCursor(CoreCursorType.Arrow, 0);

        public bool PointerPressed(PointerRoutedEventArgs e)
        {
            foreach (var handler in handlers)
            {
                if (handler.PointerPressed(e))
                {
                    Cursor = handler.Cursor;
                    return true;
                }
            }

            return false;
        }

        public bool PointerMoved(PointerRoutedEventArgs e)
        {
            foreach (var handler in handlers)
            {
                if (handler.PointerMoved(e))
                {
                    Cursor = handler.Cursor;
                    return true;
                }
            }

            return false;
        }

        public bool PointerReleased(PointerRoutedEventArgs e)
        {
            foreach (var handler in handlers)
            {
                if (handler.PointerReleased(e))
                {
                    Cursor = handler.Cursor;
                    return true;
                }
            }

            return false;
        }
    }
}
