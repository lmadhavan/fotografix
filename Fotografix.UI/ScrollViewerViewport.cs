using Fotografix.Editor;
using System.Drawing;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI
{
    public sealed class ScrollViewerViewport : Viewport
    {
        private readonly ScrollViewer scrollViewer;

        public ScrollViewerViewport(ScrollViewer scrollViewer)
        {
            this.scrollViewer = scrollViewer;
        }

        public override int Width => (int)scrollViewer.ActualWidth;
        public override int Height => (int)scrollViewer.ActualHeight;

        public override float ZoomFactor
        {
            get => scrollViewer.ZoomFactor;
            set => scrollViewer.ChangeView(null, null, value);
        }

        public override PointF ScrollOffset
        {
            get => new PointF((float)scrollViewer.HorizontalOffset, (float)scrollViewer.VerticalOffset);
            set => scrollViewer.ChangeView(value.X, value.Y, null, true);
        }
    }
}
