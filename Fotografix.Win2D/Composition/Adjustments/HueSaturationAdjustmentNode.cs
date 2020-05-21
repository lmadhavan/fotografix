using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.ComponentModel;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal sealed class HueSaturationAdjustmentNode : ColorMatrixAdjustmentNode
    {
        private readonly HueSaturationAdjustment adjustment;
        private readonly HueRotationEffect hueEffect;

        public HueSaturationAdjustmentNode(HueSaturationAdjustment adjustment)
        {
            this.adjustment = adjustment;
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;

            this.hueEffect = new HueRotationEffect();
            colorMatrixEffect.Source = hueEffect;
            UpdateHueAngle();
            UpdateColorMatrix();
        }

        public override void Dispose()
        {
            hueEffect.Dispose();

            adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
            base.Dispose();
        }

        public override ICanvasImage GetOutput(ICanvasImage input)
        {
            hueEffect.Source = input;
            return colorMatrixEffect;
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HueSaturationAdjustment.Hue))
            {
                UpdateHueAngle();
            }
            else
            {
                UpdateColorMatrix();
            }
        }

        private void UpdateHueAngle()
        {
            /* 
             * Hue:
             * 
             * When hue = [ 0, 1], angle = [0,  π]
             *      hue = [-1, 0), angle = [π, 2π)
             */
            float hue = adjustment.Hue;
            float offset = hue >= 0 ? hue : 2 + hue;
            hueEffect.Angle = (float)(offset * Math.PI);
        }

        private void UpdateColorMatrix()
        {
            float saturation = adjustment.Saturation;
            float lightness = adjustment.Lightness;

            /* 
             * Saturation:
             * 
             * gray = (r + g + b) / 3
             * dst = (1 + saturation) * src - saturation * gray
             * 
             * When saturation =  0, dst = src
             *      saturation = -1, dst = gray
             *      saturation =  1, dst = 2 * src - gray
             */
            colorMatrix.M11 = colorMatrix.M22 = colorMatrix.M33 = 1 + saturation - saturation / 3;
            colorMatrix.M21 = colorMatrix.M31 = colorMatrix.M12 = colorMatrix.M32 = colorMatrix.M13 = colorMatrix.M23 = -saturation / 3;

            /*
             * Lightness:
             * 
             * dst = src + lightness
             */
            colorMatrix.M51 = colorMatrix.M52 = colorMatrix.M53 = lightness;

            colorMatrixEffect.ColorMatrix = colorMatrix;
        }
    }
}
