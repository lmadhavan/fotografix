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

        public override bool Accept(ImageElementVisitor visitor)
        {
            if (visitor.VisitEnter(this))
            {
                Adjustment.Accept(visitor);
            }

            return visitor.VisitLeave(this);
        }
    }
}
