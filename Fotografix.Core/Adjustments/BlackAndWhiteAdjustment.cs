namespace Fotografix.Adjustments
{
    public sealed class BlackAndWhiteAdjustment : Adjustment
    {
        public override bool Accept(ImageElementVisitor visitor)
        {
            return visitor.Visit(this);
        }
    }
}