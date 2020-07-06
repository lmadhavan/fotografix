using Fotografix.Adjustments;

namespace Fotografix.Testing
{
    public class FakeAdjustment : Adjustment
    {
        private int property;

        public int Property
        {
            get => property;
            set => SetProperty(ref property, value);
        }

        public override void Apply(IAdjustmentContext adjustmentContext)
        {
        }
    }
}
