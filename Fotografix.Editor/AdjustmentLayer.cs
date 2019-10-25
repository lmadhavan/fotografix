using Fotografix.Editor.Adjustments;

namespace Fotografix.Editor
{
    public sealed class AdjustmentLayer : Layer
    {
        public AdjustmentLayer(Adjustment adjustment)
        {
            this.Adjustment = adjustment;
            this.Content = adjustment.Output;
            adjustment.PropertyChanged += (s, e) => Invalidate();
        }

        public override void Dispose()
        {
            base.Dispose();
            Adjustment.Dispose();
        }

        public Adjustment Adjustment { get; }

        protected override void OnBackgroundChanged()
        {
            base.OnBackgroundChanged();
            Adjustment.Input = Background;
        }
    }
}
