using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Adjustments
{
    internal sealed class Win2DBrightnessContrastAdjustment : BrightnessContrastAdjustment
    {
        private readonly GammaTransferEffect gammaEffect;
        private readonly ContrastEffect contrastEffect;

        public Win2DBrightnessContrastAdjustment()
        {
            this.gammaEffect = new GammaTransferEffect();
            this.contrastEffect = new ContrastEffect() { Source = gammaEffect };
        }

        public override void Dispose()
        {
            contrastEffect.Dispose();
            gammaEffect.Dispose();
            base.Dispose();
        }

        public override void Apply(IAdjustmentContext adjustmentContext)
        {
            Win2DAdjustmentContext ac = (Win2DAdjustmentContext)adjustmentContext;
            gammaEffect.Source = ac.Input;
            ac.Output = contrastEffect;
        }

        protected override void OnBrightnessChanged()
        {
            base.OnBrightnessChanged();
            UpdateGamma();
        }

        protected override void OnContrastChanged()
        {
            base.OnContrastChanged();
            UpdateContrast();
        }

        private void UpdateContrast()
        {
            contrastEffect.Contrast = Contrast;
        }

        private void UpdateGamma()
        {
            float gamma;

            if (Brightness <= 0)
            {
                // brightness values [-1, 0] map to gamma values [1, 10]
                gamma = 1 - 9 * Brightness;
            }
            else
            {
                // brightness values (0, 1] map to gamma values (1, 0]
                gamma = 1 - Brightness;
            }

            gammaEffect.RedExponent = gammaEffect.GreenExponent = gammaEffect.BlueExponent = gamma;
        }
    }
}
