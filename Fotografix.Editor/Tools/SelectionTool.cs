using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class SelectionTool : ITool
    {
        private Image image;
        private Point start;
        private bool tracking;

        public string Name => "Selection";
        public ToolCursor Cursor => ToolCursor.Crosshair;

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
            this.start = p.Location;
            this.tracking = true;
        }

        public void PointerMoved(PointerState p)
        {
            if (tracking)
            {
                Point end = p.Location;
                Rectangle rect = Rectangle.FromLTRB(start.X, start.Y, end.X, end.Y);
                image.SetSelection(rect);
            }
        }

        public void PointerReleased(PointerState p)
        {
            this.tracking = false;
        }
    }
}
