using Fotografix.Adjustments;
using System.ComponentModel;

namespace Fotografix
{
    public sealed class AdjustmentLayer : Layer
    {
        public AdjustmentLayer(IAdjustment adjustment)
        {
            this.Adjustment = adjustment;
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;
        }

        public IAdjustment Adjustment { get; }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseContentChanged();
        }
    }
}
