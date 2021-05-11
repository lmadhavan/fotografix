using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class MoveTool : ChannelTool
    {
        private Size offset;
        private bool tracking;

        public override string Name => "Move";

        private bool CanMove => ActiveChannel?.CanMove ?? false;
        public override ToolCursor Cursor => CanMove ? ToolCursor.Move : ToolCursor.Disabled;

        public override void PointerPressed(PointerState p)
        {
            if (CanMove)
            {
                Point startPos = ActiveChannel.Position;
                this.offset = new(startPos.X - p.Location.X, startPos.Y - p.Location.Y);
                this.tracking = true;
            }
        }

        public override void PointerMoved(PointerState p)
        {
            if (tracking)
            {
                ActiveChannel.Position = p.Location + offset;
            }
        }

        public override void PointerReleased(PointerState p)
        {
            this.tracking = false;
        }
    }
}
