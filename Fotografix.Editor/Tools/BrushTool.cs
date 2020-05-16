using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : ITool, IBrushToolSettings
    {
        private BrushStroke brushStroke;

        public float Size { get; set; }
        public Color Color { get; set; }

        object ITool.Settings => this;

        public void PointerPressed(PointF pt)
        {
            this.brushStroke = new BrushStroke(pt, Size, Color);
        }

        public void PointerMoved(PointF pt)
        {
            brushStroke?.AddPoint(pt);
        }

        public void PointerReleased(PointF pt)
        {
            BrushStrokeCompleted?.Invoke(this, new BrushStrokeEventArgs(brushStroke));
        }

        public event EventHandler<BrushStrokeEventArgs> BrushStrokeCompleted;
    }
}
