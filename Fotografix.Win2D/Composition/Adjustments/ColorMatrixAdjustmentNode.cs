using Fotografix.Adjustments;
using Microsoft.Graphics.Canvas.Effects;

namespace Fotografix.Win2D.Composition.Adjustments
{
    internal abstract class ColorMatrixAdjustmentNode : AdjustmentNode
    {
        protected readonly ColorMatrixEffect colorMatrixEffect;
        protected Matrix5x4 colorMatrix;

        protected ColorMatrixAdjustmentNode(Adjustment adjustment) : base(adjustment)
        {
            this.colorMatrixEffect = new ColorMatrixEffect();
            this.Output = colorMatrixEffect;

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
        }

        protected override void OnInputChanged()
        {
            colorMatrixEffect.Source = Input;
            Invalidate();
        }
    }
}
