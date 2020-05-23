using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : ITool, IBrushToolSettings
    {
        private bool enabled;
        private BrushStroke brushStroke;

        public float Size { get; set; } = 5;
        public Color Color { get; set; } = Color.White;

        object ITool.Settings => this;
        public ToolCursor Cursor => enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        public void LayerActivated(Layer layer)
        {
            this.enabled = layer.CanPaint;
        }

        public void PointerPressed(PointF pt)
        {
            if (enabled)
            {
                this.brushStroke = new BrushStroke(pt, Size, Color);
            }
        }

        public void PointerMoved(PointF pt)
        {
            brushStroke?.AddPoint(pt);
        }

        public void PointerReleased(PointF pt)
        {
            if (brushStroke != null)
            {
                BrushStrokeCompleted?.Invoke(this, new BrushStrokeEventArgs(brushStroke));
                this.brushStroke = null;
            }
        }

        public event EventHandler<BrushStrokeEventArgs> BrushStrokeCompleted;
    }
}
