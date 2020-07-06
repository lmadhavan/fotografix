using System;

namespace Fotografix.Adjustments
{
    public abstract class HueSaturationAdjustment : Adjustment, IHueSaturationAdjustment
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

                if (SetProperty(ref hue, value))
                {
                    OnHueChanged();
                }
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

                if (SetProperty(ref saturation, value))
                {
                    OnSaturationChanged();
                }
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

                if (SetProperty(ref lightness, value))
                {
                    OnLightnessChanged();
                }
            }
        }

        protected virtual void OnHueChanged()
        {
        }

        protected virtual void OnSaturationChanged()
        {
        }

        protected virtual void OnLightnessChanged()
        {
        }
    }
}
