using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public class HandTool : ITool
    {
        private Viewport viewport;
        private bool tracking;
        private Point anchor;

        public string Name => "Hand";
        public ToolCursor Cursor => tracking ? ToolCursor.ClosedHand : ToolCursor.OpenHand;

        public void Activated(Image image)
        {
            this.viewport = image.GetViewport();
        }

        public void Deactivated()
        {
            this.viewport = null;
        }

        public void PointerPressed(PointerState p)
        {
            Point pt = viewport.TransformImageToViewport(p.Location);
            this.anchor = pt + (Size)viewport.ScrollOffset;
            this.tracking = true;
        }

        public void PointerMoved(PointerState p)
        {
            if (tracking)
            {
                Point pt = viewport.TransformImageToViewport(p.Location);
                viewport.ScrollOffset = anchor - (Size)pt;
            }
        }

        public void PointerReleased(PointerState p)
        {
            this.tracking = false;
        }
    }
}