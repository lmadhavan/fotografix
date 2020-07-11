using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal sealed class BlackAndWhiteAdjustmentNode : AdjustmentNode
    {
        private readonly GrayscaleEffect grayscaleEffect;

        public BlackAndWhiteAdjustmentNode(IBlackAndWhiteAdjustment adjustment)
        {
            this.grayscaleEffect = new GrayscaleEffect();
        }

        public override void Dispose()
        {
            grayscaleEffect.Dispose();
            base.Dispose();
        }

        public override ICanvasImage GetOutput(ICanvasImage input)
        {
            grayscaleEffect.Source = input;
            return grayscaleEffect;
        }
    }
}