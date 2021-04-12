using Fotografix.Win2D.Composition.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Win2D.Composition
{
    internal sealed class AdjustmentBlendingStrategy : IBlendingStrategy
    {
        private readonly AdjustmentNode adjustmentNode;
        private readonly BlendEffectNode blendEffectNode;
        private readonly CrossFadeEffect crossFadeEffect;

        internal AdjustmentBlendingStrategy(AdjustmentNode adjustmentNode)
        {
            this.adjustmentNode = adjustmentNode;
            this.blendEffectNode = new BlendEffectNode();
            this.crossFadeEffect = new CrossFadeEffect();
        }

        public void Dispose()
        {
            crossFadeEffect.Dispose();
            blendEffectNode.Dispose();
            adjustmentNode.Dispose();
        }

        public event EventHandler Invalidated
        {
            add { }
            remove { }
        }

        public ICanvasImage Blend(Layer layer, ICanvasImage background)
        {
            if (!layer.Visible || layer.Opacity == 0 || background == null)
            {
                return background;
            }

            ICanvasImage blendOutput = BlendAdjustment(background, layer.BlendMode);
            return CrossFade(blendOutput, background, layer.Opacity);
        }

        private ICanvasImage CrossFade(ICanvasImage foreground, ICanvasImage background, float opacity)
        {
            if (opacity == 1)
            {
                return foreground;
            }

            crossFadeEffect.Source1 = background;
            crossFadeEffect.Source2 = foreground;
            crossFadeEffect.CrossFade = opacity;
            return crossFadeEffect;
        }

        private ICanvasImage BlendAdjustment(ICanvasImage background, BlendMode blendMode)
        {
            ICanvasImage adjustmentOutput = adjustmentNode.GetOutput(background);

            if (blendMode == BlendMode.Normal)
            {
                return adjustmentOutput;
            }

            return blendEffectNode.Blend(adjustmentOutput, background, blendMode);
        }
    }
}
