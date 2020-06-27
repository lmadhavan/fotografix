using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : ITool, IBrushToolSettings
    {
        private Layer activeLayer;
        private BrushStroke brushStroke;

        public float Size { get; set; }
        public Color Color { get; set; }

        object ITool.Settings => this;
        public ToolCursor Cursor => Enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        private bool Enabled => activeLayer.CanPaint;

        public void LayerActivated(Layer layer)
        {
            this.activeLayer = layer;
        }

        public void PointerPressed(IPointerEvent e)
        {
            if (Enabled)
            {
                this.brushStroke = new BrushStroke(e.Location, Size, Color);
                BrushStrokeStarted?.Invoke(this, new BrushStrokeEventArgs(activeLayer, brushStroke));
            }
        }

        public void PointerMoved(IPointerEvent e)
        {
            brushStroke?.AddPoint(e.Location);
        }

        public void PointerReleased(IPointerEvent e)
        {
            if (brushStroke != null)
            {
                BrushStrokeCompleted?.Invoke(this, new BrushStrokeEventArgs(activeLayer, brushStroke));
                this.brushStroke = null;
            }
        }

        public event EventHandler<BrushStrokeEventArgs> BrushStrokeStarted;
        public event EventHandler<BrushStrokeEventArgs> BrushStrokeCompleted;
    }
}
