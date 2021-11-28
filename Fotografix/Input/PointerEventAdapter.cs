using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Fotografix.Input
{
    public sealed class PointerEventAdapter : IPointerEventHandler
    {
        private readonly UIElement element;
        private readonly IPointerInputHandler handler;

        public PointerEventAdapter(UIElement element, IPointerInputHandler handler)
        {
            this.element = element;
            this.handler = handler;
        }

        public CoreCursor Cursor => handler.Cursor;

        public bool PointerPressed(PointerRoutedEventArgs e)
        {
            return handler.PointerPressed(e.GetCurrentPoint(element).Position);
        }

        public bool PointerMoved(PointerRoutedEventArgs e)
        {
            return handler.PointerMoved(e.GetCurrentPoint(element).Position);
        }

        public bool PointerReleased(PointerRoutedEventArgs e)
        {
            return handler.PointerReleased(e.GetCurrentPoint(element).Position);
        }
    }
}
