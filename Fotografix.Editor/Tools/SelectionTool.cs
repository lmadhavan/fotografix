using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class SelectionTool : ITool
    {
        private readonly IPointerEventListener selectHandler;
        private readonly IPointerEventListener moveHandler;

        private Image image;

        private bool tracking;
        private IPointerEventListener activeHandler;

        public string Name => "Selection";
        public ToolCursor Cursor => activeHandler.Cursor;

        public SelectionTool()
        {
            this.selectHandler = new SelectHandler(this);
            this.moveHandler = new MoveHandler(this);
            this.activeHandler = selectHandler;
        }

        public void Activated(Image image)
        {
            this.image = image;
        }

        public void Deactivated()
        {
            this.image = null;
        }

        public void PointerPressed(PointerState p)
        {
            activeHandler.PointerPressed(p);
            this.tracking = true;
        }

        public void PointerMoved(PointerState p)
        {
            if (tracking)
            {
                activeHandler.PointerMoved(p);
            }
            else
            {
                this.activeHandler = HandlerFor(p);
            }
        }

        public void PointerReleased(PointerState p)
        {
            activeHandler.PointerReleased(p);
            this.tracking = false;
        }

        private IPointerEventListener HandlerFor(PointerState p)
        {
            var selection = image.GetSelection();

            if (selection?.Contains(p.Location) ?? false)
            {
                return moveHandler;
            }
            else
            {
                return selectHandler;
            }
        }

        private sealed class SelectHandler : IPointerEventListener
        {
            private readonly SelectionTool tool;
            private Point start;

            public SelectHandler(SelectionTool tool)
            {
                this.tool = tool;
            }

            public ToolCursor Cursor => ToolCursor.Crosshair;

            public void PointerPressed(PointerState p)
            {
                this.start = p.Location;
            }

            public void PointerMoved(PointerState p)
            {
                Point end = p.Location;
                Rectangle rect = Rectangle.FromLTRB(start.X, start.Y, end.X, end.Y).Normalize();
                tool.image.SetSelection(rect);
            }

            public void PointerReleased(PointerState p)
            {
                if (p.Location == start)
                {
                    tool.image.SetSelection(null);
                }
            }
        }

        private sealed class MoveHandler : IPointerEventListener
        {
            private readonly SelectionTool tool;
            private Rectangle rect;
            private MoveTracker tracker;

            public MoveHandler(SelectionTool tool)
            {
                this.tool = tool;
            }

            public ToolCursor Cursor => ToolCursor.Move;

            public void PointerPressed(PointerState p)
            {
                this.rect = tool.image.GetSelection() ?? default;
                this.tracker = new MoveTracker(rect.Location, p);
            }

            public void PointerMoved(PointerState p)
            {
                rect.Location = tracker.ObjectPositionAt(p);
                tool.image.SetSelection(rect);
            }

            public void PointerReleased(PointerState p)
            {
            }
        }
    }
}
