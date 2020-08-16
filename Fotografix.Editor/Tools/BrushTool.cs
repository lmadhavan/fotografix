using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : ITool, IBrushToolSettings, IDrawingSurfaceListener
    {
        private readonly IBrushStrokeFactory brushStrokeFactory;

        private IDrawingSurface drawingSurface;
        private IBrushStroke brushStroke;

        public BrushTool(IBrushStrokeFactory brushStrokeFactory)
        {
            this.brushStrokeFactory = brushStrokeFactory;
        }

        public int Size { get; set; }
        public Color Color { get; set; }

        public string Name => "Brush";
        object ITool.Settings => this;
        public ToolCursor Cursor => Enabled ? ToolCursor.Crosshair : ToolCursor.Disabled;

        private bool Enabled => drawingSurface != null;

        public void DrawingSurfaceActivated(IDrawingSurface drawingSurface)
        {
            this.drawingSurface = drawingSurface;
        }

        public void PointerPressed(PointerState p)
        {
            if (Enabled)
            {
                this.brushStroke = brushStrokeFactory.CreateBrushStroke(p.Location, Size, Color);
                drawingSurface.BeginDrawing(brushStroke);
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
                drawingSurface.EndDrawing(brushStroke);
                this.brushStroke = null;
            }
        }
    }
}
