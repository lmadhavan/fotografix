using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Win2D.Adjustments
{
    internal sealed class Win2DHueSaturationAdjustment : HueSaturationAdjustment
    {
        private readonly HueRotationEffect hueEffect;
        private readonly ColorMatrixEffect colorMatrixEffect;
        private Matrix5x4 colorMatrix;

        public Win2DHueSaturationAdjustment()
        {
            this.hueEffect = new HueRotationEffect();
            this.colorMatrixEffect = new ColorMatrixEffect();
            this.colorMatrix = ColorMatrix.Identity;
            colorMatrixEffect.ColorMatrix = colorMatrix;
            colorMatrixEffect.Source = hueEffect;
        }

        public override void Dispose()
        {
            colorMatrixEffect.Dispose();
            hueEffect.Dispose();
            base.Dispose();
        }

        public override void Apply(IAdjustmentContext adjustmentContext)
        {
            Win2DAdjustmentContext ac = (Win2DAdjustmentContext)adjustmentContext;
            hueEffect.Source = ac.Input;
            ac.Output = colorMatrixEffect;
        }

        protected override void OnHueChanged()
        {
            base.OnHueChanged();
            UpdateHueAngle();
        }

        protected override void OnSaturationChanged()
        {
            base.OnSaturationChanged();
            UpdateColorMatrix();
        }

        protected override void OnLightnessChanged()
        {
            base.OnLightnessChanged();
            UpdateColorMatrix();
        }

        private void UpdateHueAngle()
        {
            /* 
             * Hue:
             * 
             * When hue = [ 0, 1], angle = [0,  π]
             *      hue = [-1, 0), angle = [π, 2π)
             */
            float offset = Hue >= 0 ? Hue : 2 + Hue;
            hueEffect.Angle = (float)(offset * Math.PI);
        }

        private void UpdateColorMatrix()
        {
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
            this.colorMatrix.M11 = this.colorMatrix.M22 = this.colorMatrix.M33 = 1 + Saturation - Saturation / 3;
            this.colorMatrix.M21 = this.colorMatrix.M31 = this.colorMatrix.M12 = this.colorMatrix.M32 = this.colorMatrix.M13 = this.colorMatrix.M23 = -Saturation / 3;

            /*
             * Lightness:
             * 
             * dst = src + lightness
             */
            this.colorMatrix.M51 = this.colorMatrix.M52 = this.colorMatrix.M53 = Lightness;

            colorMatrixEffect.ColorMatrix = colorMatrix;
        }
    }
}
