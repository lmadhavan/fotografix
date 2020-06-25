using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public class HandTool : ITool
    {
        private bool active;
        private PointF lastPoint;

        object ITool.Settings => this;

        public ToolCursor Cursor => ToolCursor.Hand;

        public void LayerActivated(Layer layer)
        {
        }

        public void PointerPressed(PointF pt)
        {
            this.active = true;
            this.lastPoint = pt;
        }

        public void PointerMoved(PointF pt)
        {
            if (active)
            {
                PointF scrollDelta = new PointF(lastPoint.X - pt.X, lastPoint.Y - pt.Y);
                this.lastPoint = pt;

                ViewportScrollRequested?.Invoke(this, new ViewportScrollRequestedEventArgs(scrollDelta));
            }
        }

        public void PointerReleased(PointF pt)
        {
            this.active = false;
        }

        public event EventHandler<ViewportScrollRequestedEventArgs> ViewportScrollRequested;
    }
}