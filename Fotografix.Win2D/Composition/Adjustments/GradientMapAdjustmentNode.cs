using System.ComponentModel;
using System.Drawing;
using Fotografix.Adjustments;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal sealed class GradientMapAdjustmentNode : ColorMatrixAdjustmentNode
    {
        private readonly IGradientMapAdjustment adjustment;

        public GradientMapAdjustmentNode(IGradientMapAdjustment adjustment)
        {
            this.adjustment = adjustment;
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;

            UpdateColorMatrix();
        }

        public override void Dispose()
        {
            adjustment.PropertyChanged -= OnAdjustmentPropertyChanged;
            base.Dispose();
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateColorMatrix();
        }

        private void UpdateColorMatrix()
        {
            Color shadows = adjustment.Shadows;
            Color highlights = adjustment.Highlights;

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
