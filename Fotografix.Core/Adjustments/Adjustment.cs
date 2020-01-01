namespace Fotografix.Adjustments
{
    public abstract class Adjustment : NotifyPropertyChangedBase
    {
        internal Adjustment()
        {
        }

        public abstract void Accept(AdjustmentVisitor visitor);
    }
}
