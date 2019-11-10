using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Composition
{
    public sealed class AdjustmentLayer : Layer
    {
        private readonly CrossFadeEffect crossFadeEffect;

        public AdjustmentLayer(Adjustment adjustment)
        {
            this.crossFadeEffect = new CrossFadeEffect();

            this.Adjustment = adjustment;
            adjustment.PropertyChanged += (s, e) => Invalidate();
            UpdateOutput();
        }

        public override void Dispose()
        {
            base.Dispose();
            crossFadeEffect.Dispose();
            Adjustment.Dispose();
        }

        public Adjustment Adjustment { get; }

        protected override ICanvasImage ResolveOutput(ICanvasImage background)
        {
            if (!Visible || Opacity == 0 || background == null)
            {
                return background;
            }

            ICanvasImage blendOutput = BlendAdjustment(background);

            if (Opacity == 1)
            {
                return blendOutput;
            }

            crossFadeEffect.Source1 = background;
            crossFadeEffect.Source2 = blendOutput;
            crossFadeEffect.CrossFade = Opacity;
            return crossFadeEffect;
        }

        private ICanvasImage BlendAdjustment(ICanvasImage background)
        {
            Adjustment.Input = background;

            if (BlendMode == BlendMode.Normal)
            {
                return Adjustment.Output;
            }

            return Blend(Adjustment.Output, background);
        }
    }
}
