namespace Fotografix.Editor.Tools
{
    public sealed class MoveTool : ChannelTool
    {
        private MoveTracker tracker;
        private bool tracking;

        public override string Name => "Move";

        private bool CanMove => ActiveChannel?.CanMove ?? false;
        public override ToolCursor Cursor => CanMove ? ToolCursor.Move : ToolCursor.Disabled;

        public override void PointerPressed(PointerState p)
        {
            if (CanMove)
            {
                this.tracker = new MoveTracker(ActiveChannel.Position, p);
                this.tracking = true;
            }
        }

        public override void PointerMoved(PointerState p)
        {
            if (tracking)
            {
                ActiveChannel.Position = tracker.ObjectPositionAt(p);
            }
        }

        public override void PointerReleased(PointerState p)
        {
            this.tracking = false;
        }
    }
}
