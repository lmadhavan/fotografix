using Fotografix.Drawing;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public sealed class GradientTool : DrawingTool<IGradient>, IGradientToolSettings
    {
        private readonly IGradientFactory gradientFactory;

        public GradientTool(IGradientFactory gradientFactory)
        {
            this.gradientFactory = gradientFactory;
        }

        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public override string Name => "Gradient";

        protected override object Settings => this;

        protected override IGradient CreateDrawable(PointerState p)
        {
            return gradientFactory.CreateLinearGradient(StartColor, EndColor, p.Location);
        }

        protected override void UpdateDrawable(IGradient gradient, PointerState p)
        {
            gradient.SetEndPoint(p.Location);
        }
    }
}
