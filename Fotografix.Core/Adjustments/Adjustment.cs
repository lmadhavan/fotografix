namespace Fotografix.Adjustments
{
    public abstract class Adjustment : ImageElement
    {
        internal Adjustment()
        {
        }

        public abstract void Accept(AdjustmentVisitor visitor);
    }
}
