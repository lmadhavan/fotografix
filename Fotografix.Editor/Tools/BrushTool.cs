using System;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : ITool, ILayerActivationListener, IBrushToolSettings
    {
        private Layer activeLayer;
        private BrushStroke brushStroke;

        public float Size { get; set; }
        public Color Color { get; set; }

        public string Name => "Brush";
        object ITool.Settings => this;
        public ToolCursor Cursor => Enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        private bool Enabled => activeLayer.CanPaint;

        public void LayerActivated(Layer layer)
        {
            this.activeLayer = layer;
        }

        public void PointerPressed(PointerState p)
        {
            if (Enabled)
            {
                this.brushStroke = new BrushStroke(p.Location, Size, Color);
                BrushStrokeStarted?.Invoke(this, new BrushStrokeEventArgs(activeLayer, brushStroke));
            }
        }

        public void PointerMoved(PointerState p)
        {
            brushStroke?.AddPoint(p.Location);
        }

        public void PointerReleased(PointerState p)
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
