using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Adjustments
{
    internal sealed class Win2DBlackAndWhiteAdjustment : BlackAndWhiteAdjustment
    {
        private readonly GrayscaleEffect grayscaleEffect;

        public Win2DBlackAndWhiteAdjustment()
        {
            this.grayscaleEffect = new GrayscaleEffect();
        }

        public override void Dispose()
        {
            grayscaleEffect.Dispose();
            base.Dispose();
        }

        public override void Apply(IAdjustmentContext adjustmentContext)
        {
            Win2DAdjustmentContext ac = (Win2DAdjustmentContext)adjustmentContext;
            grayscaleEffect.Source = ac.Input;
            ac.Output = grayscaleEffect;
        }
    }
}