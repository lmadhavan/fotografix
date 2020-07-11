using System;

namespace Fotografix.Adjustments
{
    public sealed class HueSaturationAdjustment : Adjustment
    {
        private float hue;
        private float saturation;
        private float lightness;

        public float Hue
        {
            get
            {
                return hue;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref hue, value);
            }
        }

        public float Saturation
        {
            get
            {
                return saturation;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref saturation, value);
            }
        }

        public float Lightness
        {
            get
            {
                return lightness;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref lightness, value);
            }
        }

        public override void Accept(AdjustmentVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
