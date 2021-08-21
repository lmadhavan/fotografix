using Fotografix.Drawing;
using Fotografix.Editor.Colors;

namespace Fotografix.Editor.Tools
{
    public sealed class BrushTool : DrawingTool<BrushStroke>, IBrushToolControls
    {
        private readonly IColorProvider colorProvider;

        public BrushTool(IColorProvider colorProvider, AsyncCommand drawCommand) : base(drawCommand)
        {
            this.colorProvider = colorProvider;
        }

        public int Size { get; set; }

        public override string Name => "Brush";

        protected override BrushStroke CreateDrawable(Image image, PointerState p)
        {
            return new BrushStroke(p.Location, Size, colorProvider.ForegroundColor);
        }

        protected override void UpdateDrawable(BrushStroke brushStroke, PointerState p)
        {
            brushStroke.AddPoint(p.Location);
        }
    }
}
