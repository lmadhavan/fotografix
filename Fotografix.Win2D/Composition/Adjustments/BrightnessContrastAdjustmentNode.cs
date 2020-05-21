using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal sealed class BrightnessContrastAdjustmentNode : AdjustmentNode
    {
        private readonly BrightnessContrastAdjustment adjustment;
        private readonly GammaTransferEffect gammaEffect;
        private readonly ContrastEffect contrastEffect;

        public BrightnessContrastAdjustmentNode(BrightnessContrastAdjustment adjustment)
        {
            this.adjustment = adjustment;
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;

            this.gammaEffect = new GammaTransferEffect();
            this.contrastEffect = new ContrastEffect() { Source = gammaEffect };

            UpdateGamma();
            UpdateContrast();
        }

        public override void Dispose()
        {
            contrastEffect.Dispose();
            gammaEffect.Dispose();

            adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
            base.Dispose();
        }

        public override ICanvasImage GetOutput(ICanvasImage input)
        {
            gammaEffect.Source = input;
            return contrastEffect;
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BrightnessContrastAdjustment.Brightness):
                    UpdateGamma();
                    break;

                case nameof(BrightnessContrastAdjustment.Contrast):
                    UpdateContrast();
                    break;
            }
        }

        private void UpdateContrast()
        {
            contrastEffect.Contrast = adjustment.Contrast;
        }

        private void UpdateGamma()
        {
            float brightness = adjustment.Brightness;
            float gamma;

            if (brightness <= 0)
            {
                // brightness values [-1, 0] map to gamma values [1, 10]
                gamma = 1 - 9 * brightness;
            }
            else
            {
                // brightness values (0, 1] map to gamma values (1, 0]
                gamma = 1 - brightness;
            }

            gammaEffect.RedExponent = gammaEffect.GreenExponent = gammaEffect.BlueExponent = gamma;
        }
    }
}
