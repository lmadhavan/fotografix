using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : DrawingTool<IBrushStroke>, IBrushToolSettings
    {
        private readonly IBrushStrokeFactory brushStrokeFactory;

        public BrushTool(IBrushStrokeFactory brushStrokeFactory)
        {
            this.brushStrokeFactory = brushStrokeFactory;
        }

        public int Size { get; set; }
        public Color Color { get; set; }

        public override string Name => "Brush";
        protected override object Settings => this;

        protected override IBrushStroke CreateDrawable(PointerState p)
        {
            return brushStrokeFactory.CreateBrushStroke(p.Location, Size, Color);
        }

        protected override void UpdateDrawable(IBrushStroke brushStroke, PointerState p)
        {
            brushStroke.AddPoint(p.Location);
        }
    }
}
