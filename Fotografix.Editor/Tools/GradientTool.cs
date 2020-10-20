using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class GradientTool : DrawingTool<LinearGradient>, IGradientToolControls
    {
        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public override string Name => "Gradient";

        protected override LinearGradient CreateDrawable(PointerState p)
        {
            return new LinearGradient(StartColor, EndColor, p.Location);
        }

        protected override void UpdateDrawable(LinearGradient gradient, PointerState p)
        {
            gradient.SetEndPoint(p.Location);
        }
    }
}
