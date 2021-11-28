using Fotografix.Input;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Fotografix
{
    public sealed class ViewportPanHandler : IPointerEventHandler
    {
        private readonly ScrollViewer viewport;
        private bool tracking;
        private Point startPosition;
        private Point startOffset;

        public ViewportPanHandler(ScrollViewer viewport)
        {
            this.viewport = viewport;
        }

        public CoreCursor Cursor { get; } = new CoreCursor(CoreCursorType.Arrow, 0);

        public bool PointerPressed(PointerRoutedEventArgs e)
        {
            if (!tracking && e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                this.tracking = true;
                this.startPosition = e.GetCurrentPoint(viewport).Position;
                this.startOffset = new Point(viewport.HorizontalOffset, viewport.VerticalOffset);
                return true;
            }

            return false;
        }

        public bool PointerMoved(PointerRoutedEventArgs e)
        {
            if (tracking)
            {
                var currentPosition = e.GetCurrentPoint(viewport).Position;

                viewport.ChangeView(
                    horizontalOffset: startOffset.X + startPosition.X - currentPosition.X,
                    verticalOffset: startOffset.Y + startPosition.Y - currentPosition.Y,
                    zoomFactor: null,
                    disableAnimation: true
                );
            }

            return true;
        }

        public bool PointerReleased(PointerRoutedEventArgs e)
        {
            this.tracking = false;
            return true;
        }
    }
}
