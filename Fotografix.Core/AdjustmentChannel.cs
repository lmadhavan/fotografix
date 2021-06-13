using Fotografix.Adjustments;
using Fotografix.Drawing;
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

        public override void Crop(Rectangle rectangle)
        {
        }

        public override void Scale(PointF scaleFactor, IDrawingContextFactory drawingContextFactory)
        {
        }
    }
}
