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

        public string Name => "Hand";
        public ToolCursor Cursor => scrollAnchor != null ? ToolCursor.ClosedHand : ToolCursor.OpenHand;

        public void Activated(Image image)
        {
        }

        public void Deactivated()
        {
        }

        public void PointerPressed(PointerState p)
        {
            this.scrollAnchor = new ScrollAnchor(viewport);
            this.start = p.ViewportLocation;
        }

        public void PointerMoved(PointerState p)
        {
            if (scrollAnchor != null)
            {
                PointF current = p.ViewportLocation;
                PointF delta = new PointF(start.X - current.X, start.Y - current.Y);
                scrollAnchor.ScrollContentTo(delta);
            }
        }

        public void PointerReleased(PointerState p)
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