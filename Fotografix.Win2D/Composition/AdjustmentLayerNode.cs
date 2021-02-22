using Fotografix.Win2D.Composition.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Drawing;

namespace Fotografix.Win2D.Composition
{
    internal sealed class AdjustmentLayerNode : LayerNode
    {
        private readonly AdjustmentLayer layer;
        private readonly CrossFadeEffect crossFadeEffect;
        private AdjustmentNode adjustmentNode;

        public AdjustmentLayerNode(AdjustmentLayer layer, NodeFactory nodeFactory) : base(layer, nodeFactory)
        {
            this.layer = layer;
            this.crossFadeEffect = new CrossFadeEffect();
            RegisterAdjustment();
            UpdateOutput();
        }

        public override void Dispose()
        {
            UnregisterAdjustment();
            crossFadeEffect.Dispose();
            base.Dispose();
        }

        protected override Rectangle Bounds => Rectangle.Empty;

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
            ICanvasImage adjustmentOutput = adjustmentNode.GetOutput(background);

            if (layer.BlendMode == BlendMode.Normal)
            {
                return adjustmentOutput;
            }

            return Blend(adjustmentOutput, background);
        }

        private void RegisterAdjustment()
        {
            this.adjustmentNode = NodeFactory.CreateAdjustmentNode(layer.Adjustment);
        }

        private void UnregisterAdjustment()
        {
            adjustmentNode.Dispose();
        }

        private void OnAdjustmentOutputChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }
    }
}
