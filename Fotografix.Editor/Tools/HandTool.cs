using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public class HandTool : ITool
    {
        private readonly Viewport viewport;
        private ScrollAnchor scrollAnchor;
        private PointF start;

        public HandTool(Viewport viewport)
        {
            this.viewport = viewport;
        }

        object ITool.Settings => this;

        public ToolCursor Cursor => ToolCursor.Hand;

        public void LayerActivated(Layer layer)
        {
        }

        public void PointerPressed(IPointerEvent e)
        {
            this.scrollAnchor = new ScrollAnchor(viewport);
            this.start = e.ViewportLocation;
        }

        public void PointerMoved(IPointerEvent e)
        {
            if (scrollAnchor != null)
            {
                PointF current = e.ViewportLocation;
                PointF delta = new PointF(start.X - current.X, start.Y - current.Y);
                scrollAnchor.ScrollContentTo(delta);
            }
        }

        public void PointerReleased(IPointerEvent e)
        {
            this.scrollAnchor = null;
        }

        private sealed class ScrollAnchor
        {
            private readonly Viewport viewport;
            private readonly PointF anchorPoint;

            public ScrollAnchor(Viewport viewport)
            {
                this.viewport = viewport;
                this.anchorPoint = viewport.ScrollOffset;
            }

            public void ScrollContentTo(PointF distanceFromAnchor)
            {
                viewport.ScrollOffset = new PointF(
                    anchorPoint.X + distanceFromAnchor.X,
                    anchorPoint.Y + distanceFromAnchor.Y
                );
            }
        }
    }
}