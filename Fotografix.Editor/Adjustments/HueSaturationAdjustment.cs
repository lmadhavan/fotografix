using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.Graphics.Effects;

namespace Fotografix.Editor.Adjustments
{
    public sealed class HueSaturationAdjustment : Adjustment
    {
        private readonly HueRotationEffect hueEffect;
        private readonly ColorMatrixEffect colorMatrixEffect;
        private Matrix5x4 colorMatrix;

        private float hue;
        private float saturation;
        private float lightness;

        public HueSaturationAdjustment()
        {
            this.Name = "Hue/Saturation";

            this.hueEffect = new HueRotationEffect();
            this.colorMatrixEffect = new ColorMatrixEffect() { Source = hueEffect };

            this.colorMatrix = new Matrix5x4
            { 
                M11 = 1, M12 = 0, M13 = 0, M14 = 0,
                M21 = 0, M22 = 1, M23 = 0, M24 = 0,
                M31 = 0, M32 = 0, M33 = 1, M34 = 0,
                M41 = 0, M42 = 0, M43 = 0, M44 = 1,
                M51 = 0, M52 = 0, M53 = 0, M54 = 0
            };
        }

        public override void Dispose()
        {
            colorMatrixEffect.Dispose();
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

        internal override IGraphicsEffectSource Input
        {
            get
            {
                return hueEffect.Source;
            }

            set
            {
                hueEffect.Source = value;
            }
        }

        internal override ICanvasImage Output => colorMatrixEffect;
    }
}
