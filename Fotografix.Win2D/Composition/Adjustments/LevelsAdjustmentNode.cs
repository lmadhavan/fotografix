using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal sealed class LevelsAdjustmentNode : AdjustmentNode
    {
        private readonly LevelsAdjustment adjustment;
        private readonly GammaTransferEffect gammaEffect;

        public LevelsAdjustmentNode(LevelsAdjustment adjustment)
        {
            this.adjustment = adjustment;
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;

            this.gammaEffect = new GammaTransferEffect();
            UpdateCoefficients();
        }

        public override void Dispose()
        {
            gammaEffect.Dispose();
            adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
            base.Dispose();
        }

        public override ICanvasImage GetOutput(ICanvasImage input)
        {
            gammaEffect.Source = input;
            return gammaEffect;
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateCoefficients();
        }

        private void UpdateCoefficients()
        {
            /*
             * dst = [(src^(1/ig) - ibp) / (iwp - ibp)] * (owp - obp) + obp
             *     = src^(1/ig) * x - ibp * x + obp
             * 
             * where x = (owp - obp) / (iwp - ibp)
             */

            float x = (adjustment.OutputWhitePoint - adjustment.OutputBlackPoint) / (adjustment.InputWhitePoint - adjustment.InputBlackPoint);

            gammaEffect.RedExponent = gammaEffect.GreenExponent = gammaEffect.BlueExponent = 1 / adjustment.InputGamma;
            gammaEffect.RedAmplitude = gammaEffect.GreenAmplitude = gammaEffect.BlueAmplitude = x;
            gammaEffect.RedOffset = gammaEffect.GreenOffset = gammaEffect.BlueOffset = -adjustment.InputBlackPoint * x + adjustment.OutputBlackPoint;
        }
    }
}
