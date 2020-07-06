using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Adjustments
{
    internal sealed class Win2DGradientMapAdjustment : GradientMapAdjustment
    {
        private readonly ColorMatrixEffect colorMatrixEffect;
        private Matrix5x4 colorMatrix;

        public Win2DGradientMapAdjustment()
        {
            this.colorMatrixEffect = new ColorMatrixEffect();
            this.colorMatrix = ColorMatrix.Identity;
            colorMatrixEffect.ColorMatrix = colorMatrix;
        }

        public override void Dispose()
        {
            colorMatrixEffect.Dispose();
            base.Dispose();
        }

        public override void Apply(IAdjustmentContext adjustmentContext)
        {
            Win2DAdjustmentContext ac = (Win2DAdjustmentContext)adjustmentContext;
            colorMatrixEffect.Source = ac.Input;
            ac.Output = colorMatrixEffect;
        }

        protected override void OnShadowsChanged()
        {
            base.OnShadowsChanged();
            UpdateColorMatrix();
        }

        protected override void OnHighlightsChanged()
        {
            base.OnHighlightsChanged();
            UpdateColorMatrix();
        }

        private void UpdateColorMatrix()
        {
            /*
             * gray = (r + g + b) / 3
             * dst = (1 - gray) * shadows + gray * highlights
             *     = shadows + (highlights - shadows) * gray
             */

            this.colorMatrix.M51 = Shadows.R / 255f;
            this.colorMatrix.M11 = this.colorMatrix.M21 = this.colorMatrix.M31 = (Highlights.R - Shadows.R) / 255f / 3;

            this.colorMatrix.M52 = Shadows.G / 255f;
            this.colorMatrix.M12 = this.colorMatrix.M22 = this.colorMatrix.M32 = (Highlights.G - Shadows.G) / 255f / 3;

            this.colorMatrix.M53 = Shadows.B / 255f;
            this.colorMatrix.M13 = this.colorMatrix.M23 = this.colorMatrix.M33 = (Highlights.B - Shadows.B) / 255f / 3;

            colorMatrixEffect.ColorMatrix = colorMatrix;
        }
    }
}
