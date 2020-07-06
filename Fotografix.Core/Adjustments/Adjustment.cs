namespace Fotografix.Adjustments
{
    public abstract class Adjustment : NotifyPropertyChangedBase, IAdjustment
    {
        public virtual void Dispose()
        {
        }

        public abstract void Apply(IAdjustmentContext adjustmentContext);
    }
}
