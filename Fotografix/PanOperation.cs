using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Fotografix
{
    public class PanOperation
    {
        private readonly ScrollViewer viewport;
        private readonly Point startPosition;
        private readonly Point startOffset;

        public PanOperation(ScrollViewer viewport, PointerRoutedEventArgs startEvent)
        {
            this.viewport = viewport;
            this.startPosition = startEvent.GetCurrentPoint(viewport).Position;
            this.startOffset = new Point(viewport.HorizontalOffset, viewport.VerticalOffset);
        }

        public void Track(PointerRoutedEventArgs moveEvent)
        {
            var currentPosition = moveEvent.GetCurrentPoint(viewport).Position;

            viewport.ChangeView(
                horizontalOffset: startOffset.X + startPosition.X - currentPosition.X,
                verticalOffset: startOffset.Y + startPosition.Y - currentPosition.Y,
                zoomFactor: null,
                disableAnimation: true
            );
        }
    }
}
