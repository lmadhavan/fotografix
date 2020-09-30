using Fotografix.Adjustments;

namespace Fotografix
{
    public sealed class AdjustmentLayer : Layer
    {
        public AdjustmentLayer(Adjustment adjustment)
        {
            this.Adjustment = adjustment;
        }

        public Adjustment Adjustment { get; }

        public override void Accept(LayerVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
