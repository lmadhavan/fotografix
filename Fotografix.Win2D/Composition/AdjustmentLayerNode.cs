using Fotografix.Win2D.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition
{
    internal sealed class AdjustmentLayerNode : LayerNode
    {
        private readonly AdjustmentLayer layer;
        private readonly CrossFadeEffect crossFadeEffect;

        public AdjustmentLayerNode(AdjustmentLayer layer) : base(layer)
        {
            this.layer = layer;
            this.crossFadeEffect = new CrossFadeEffect();
            UpdateOutput();
        }

        public override void Dispose()
        {
            crossFadeEffect.Dispose();
            base.Dispose();
        }

        protected override ICanvasImage ResolveOutput(ICanvasImage background)
        {
            if (!layer.Visible || layer.Opacity == 0 || background == null)
            {
                return background;
            }

            ICanvasImage blendOutput = BlendAdjustment(background);

            if (layer.Opacity == 1)
            {
                return blendOutput;
            }

            crossFadeEffect.Source1 = background;
            crossFadeEffect.Source2 = blendOutput;
            crossFadeEffect.CrossFade = layer.Opacity;
            return crossFadeEffect;
        }

        private ICanvasImage BlendAdjustment(ICanvasImage background)
        {
            Win2DAdjustmentContext ac = new Win2DAdjustmentContext(background);
            layer.Adjustment.Apply(ac);

            if (layer.BlendMode == BlendMode.Normal)
            {
                return ac.Output;
            }

            return Blend(ac.Output, background);
        }
    }
}
