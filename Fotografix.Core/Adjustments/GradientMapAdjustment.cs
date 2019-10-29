using Windows.UI;

namespace Fotografix.Adjustments
{
    public sealed class GradientMapAdjustment : ColorMatrixAdjustment
    {
        private Color shadows;
        private Color highlights;

        public GradientMapAdjustment()
        {
            this.shadows = Colors.Black;
            this.highlights = Colors.White;
            UpdateColorMatrix();
        }

        public Color Shadows
        {
            get
            {
                return shadows;
            }

            set
            {
                if (SetValue(ref shadows, value))
                {
                    UpdateColorMatrix();
                }
            }
        }

        public Color Highlights
        {
            get
            {
                return highlights;
            }

            set
            {
                if (SetValue(ref highlights, value))
                {
                    UpdateColorMatrix();
                }
            }
        }

        private void UpdateColorMatrix()
        {
            /*
             * gray = (r + g + b) / 3
             * dst = (1 - gray) * shadows + gray * highlights
             *     = shadows + (highlights - shadows) * gray
             */

            colorMatrix.M51 = shadows.R / 255f;
            colorMatrix.M11 = colorMatrix.M21 = colorMatrix.M31 = (highlights.R - shadows.R) / 255f / 3;
            
            colorMatrix.M52 = shadows.G / 255f;
            colorMatrix.M12 = colorMatrix.M22 = colorMatrix.M32 = (highlights.G - shadows.G) / 255f / 3;

            colorMatrix.M53 = shadows.B / 255f;
            colorMatrix.M13 = colorMatrix.M23 = colorMatrix.M33 = (highlights.B - shadows.B) / 255f / 3;

            colorMatrixEffect.ColorMatrix = colorMatrix;
        }
    }
}
