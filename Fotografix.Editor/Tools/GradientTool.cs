using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class GradientTool : DrawingTool<LinearGradient>, IGradientToolControls
    {
        public GradientTool(IDocumentCommand drawCommand) : base(drawCommand)
        {
        }

        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public override string Name => "Gradient";

        protected override LinearGradient CreateDrawable(Image image, PointerState p)
        {
            Rectangle bounds = new Rectangle(Point.Empty, image.Size);
            return new LinearGradient(bounds, StartColor, EndColor) { StartPoint = p.Location };
        }

        protected override void UpdateDrawable(LinearGradient gradient, PointerState p)
        {
            gradient.EndPoint = p.Location;
        }
    }
}
