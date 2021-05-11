using Fotografix.Adjustments;
using System;
using System.Drawing;

namespace Fotografix
{
    public sealed class AdjustmentChannel : Channel
    {
        public AdjustmentChannel(Adjustment adjustment)
        {
            this.Adjustment = adjustment ?? throw new ArgumentNullException();
            AddChild(adjustment);
        }

        public override ImageElement Content => Adjustment;

        public Adjustment Adjustment { get; }

        internal override void Crop(Rectangle rectangle)
        {
        }

        internal override void Scale(PointF scaleFactor, IBitmapResamplingStrategy resamplingStrategy)
        {
        }
    }
}
