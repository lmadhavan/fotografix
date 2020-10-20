using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : DrawingTool<BrushStroke>, IBrushToolControls
    {
        public int Size { get; set; }
        public Color Color { get; set; }

        public override string Name => "Brush";

        protected override BrushStroke CreateDrawable(PointerState p)
        {
            return new BrushStroke(p.Location, Size, Color);
        }

        protected override void UpdateDrawable(BrushStroke brushStroke, PointerState p)
        {
            brushStroke.AddPoint(p.Location);
        }
    }
}
