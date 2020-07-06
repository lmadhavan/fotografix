using Fotografix.Adjustments;

namespace Fotografix.History.Adjustments
{
    public abstract class ChangeTrackingAdjustment<T> : PropertyChangeTrackingBase<T>, IAdjustment where T : IAdjustment
    {
        protected ChangeTrackingAdjustment(T target, IChangeTracker changeTracker) : base(target, changeTracker)
        {
        }

        public void Dispose()
        {
            Target.Dispose();
        }

        public void Apply(IAdjustmentContext adjustmentContext)
        {
            Target.Apply(adjustmentContext);
        }
    }
}
