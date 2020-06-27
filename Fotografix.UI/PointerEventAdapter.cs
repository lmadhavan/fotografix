using Fotografix.Editor;
using System.Drawing;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Fotografix.UI
{
    internal sealed class PointerEventAdapter : IPointerEvent
    {
        private readonly PointerRoutedEventArgs e;
        private readonly UIElement canvas;
        private readonly UIElement viewport;

        public PointerEventAdapter(PointerRoutedEventArgs e, UIElement canvas, UIElement viewport)
        {
            this.e = e;
            this.canvas = canvas;
            this.viewport = viewport;
        }

        public Point Location
        {
            get
            {
                PointerPoint pt = e.GetCurrentPoint(canvas);
                return new Point((int)pt.Position.X, (int)pt.Position.Y);
            }
        }

        public PointF ViewportLocation
        {
            get
            {
                PointerPoint pt = e.GetCurrentPoint(viewport);
                return new PointF((float)pt.Position.X, (float)pt.Position.Y);
            }
        }
    }
}