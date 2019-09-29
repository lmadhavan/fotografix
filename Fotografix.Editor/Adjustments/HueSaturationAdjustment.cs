using Microsoft.Graphics.Canvas.Effects;
using System;

namespace Fotografix.Editor.Adjustments
{
    public sealed class HueSaturationAdjustment : ColorMatrixAdjustment
    {
        private readonly HueRotationEffect hueEffect;

        private float hue;
        private float saturation;
        private float lightness;

        public HueSaturationAdjustment()
        {
            this.hueEffect = new HueRotationEffect();
            colorMatrixEffect.Source = hueEffect;
        }

        public override void Dispose()
        {
            base.Dispose();
            hueEffect.Dispose();
        }

        public float Hue
        {
            get
            {
                return hue;
            }

            set
            {
                if (SetValue(ref hue, value))
                {
                    /* 
                     * Hue:
                     * 
                     * When hue = [ 0, 1], angle = [0,  π]
                     *      hue = [-1, 0), angle = [π, 2π)
                     */
                    float offset = hue >= 0 ? hue : 2 + hue;
                    hueEffect.Angle = (float)(offset * Math.PI);
                }
            }
        }

        public float Saturation
        {
            get
            {
                return saturation;
            }

            set
            {
                if (SetValue(ref saturation, value))
                {
                    UpdateColorMatrix();
                }
            }
        }

        public float Lightness
        {
            get
            {
                return lightness;
            }

            set
            {
                if (SetValue(ref lightness, value))
                {
                    UpdateColorMatrix();
                }
            }
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

        protected override void OnInputChanged()
        {
            hueEffect.Source = Input;
        }
    }
}
