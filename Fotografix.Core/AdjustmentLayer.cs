using Fotografix.Adjustments;

namespace Fotografix
{
    public sealed class AdjustmentLayer : Layer
    {
        public AdjustmentLayer(Adjustment adjustment)
        {
            AddChild(adjustment);
            this.Adjustment = adjustment;
        }

        public Adjustment Adjustment { get; }

        public override ImageElement Content => Adjustment;
    }
}
