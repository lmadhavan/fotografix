using Fotografix.Adjustments;
using System.ComponentModel;

namespace Fotografix
{
    public sealed class AdjustmentLayer : Layer
    {
        public AdjustmentLayer(Adjustment adjustment)
        {
            this.Adjustment = adjustment;
            adjustment.PropertyChanged += OnAdjustmentPropertyChanged;
        }

        public Adjustment Adjustment { get; }

        public override void Accept(LayerVisitor visitor)
        {
            visitor.Visit(this);
        }

        private void OnAdjustmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseContentChanged();
        }
    }
}
