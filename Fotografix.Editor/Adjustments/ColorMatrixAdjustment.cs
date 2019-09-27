using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Editor.Adjustments
{
    public abstract class ColorMatrixAdjustment : Adjustment
    {
        protected readonly ColorMatrixEffect colorMatrixEffect;
        protected Matrix5x4 colorMatrix;

        protected ColorMatrixAdjustment()
        {
            this.colorMatrixEffect = new ColorMatrixEffect();
            this.RawOutput = colorMatrixEffect;

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
            base.Dispose();
            colorMatrixEffect.Dispose();
        }

        protected override void OnInputChanged()
        {
            colorMatrixEffect.Source = Input;
        }
    }
}
