using Fotografix.Adjustments;
using System;

namespace Fotografix
{
    public sealed class AdjustmentLayer : Layer
    {
        private Adjustment adjustment;

        public AdjustmentLayer(Adjustment adjustment)
        {
            this.Adjustment = adjustment;
        }

        public Adjustment Adjustment
        {
            get
            {
                return adjustment;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                SetProperty(ref adjustment, value);
            }
        }

        public override void Accept(ILayerVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
