using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal sealed class BlackAndWhiteAdjustmentNode : AdjustmentNode
    {
        private readonly GrayscaleEffect grayscaleEffect;

        public BlackAndWhiteAdjustmentNode(BlackAndWhiteAdjustment adjustment) : base(adjustment)
        {
            this.grayscaleEffect = new GrayscaleEffect();
            this.Output = grayscaleEffect;
        }

        public override void Dispose()
        {
            grayscaleEffect.Dispose();
            base.Dispose();
        }

        protected override void OnInputChanged()
        {
            grayscaleEffect.Source = Input;
            Invalidate();
        }
    }
}