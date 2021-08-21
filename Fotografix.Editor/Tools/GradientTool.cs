using Fotografix.Drawing;
using Fotografix.Editor.Colors;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class GradientTool : DrawingTool<LinearGradient>
    {
        private readonly IColorProvider colorProvider;

        public GradientTool(IColorProvider colorProvider, AsyncCommand drawCommand) : base(drawCommand)
        {
            this.colorProvider = colorProvider;
        }

        public override string Name => "Gradient";

        protected override LinearGradient CreateDrawable(Image image, PointerState p)
        {
            Rectangle bounds = new Rectangle(Point.Empty, image.Size);
            return new LinearGradient(bounds, colorProvider.ForegroundColor, colorProvider.BackgroundColor) { StartPoint = p.Location };
        }

        protected override void UpdateDrawable(LinearGradient gradient, PointerState p)
        {
            gradient.EndPoint = p.Location;
        }
    }
}
