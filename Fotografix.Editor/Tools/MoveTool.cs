using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class MoveTool : BitmapTool
    {
        private Size offset;
        private bool tracking;

        public override string Name => "Move";

        public override ToolCursor Cursor => ActiveBitmap != null ? ToolCursor.Move : ToolCursor.Disabled;

        public override void PointerPressed(PointerState p)
        {
            if (ActiveBitmap != null)
            {
                Point startPos = ActiveBitmap.Position;
                this.offset = new(startPos.X - p.Location.X, startPos.Y - p.Location.Y);
                this.tracking = true;
            }
        }

        public override void PointerMoved(PointerState p)
        {
            if (tracking)
            {
                ActiveBitmap.Position = p.Location + offset;
            }
        }

        public override void PointerReleased(PointerState p)
        {
            this.tracking = false;
        }
    }
}
