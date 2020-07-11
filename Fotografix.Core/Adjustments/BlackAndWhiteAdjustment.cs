namespace Fotografix.Adjustments
{
    public sealed class BlackAndWhiteAdjustment : Adjustment
    {
        public override void Accept(AdjustmentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}