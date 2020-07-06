using System;

namespace Fotografix.Adjustments
{
    public abstract class BrightnessContrastAdjustment : Adjustment, IBrightnessContrastAdjustment
    {
        private float brightness;
        private float contrast;

        public float Brightness
        {
            get
            {
                return brightness;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (SetProperty(ref brightness, value))
                {
                    OnBrightnessChanged();
                }
            }
        }

        public float Contrast
        {
            get
            {
                return contrast;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (SetProperty(ref contrast, value))
                {
                    OnContrastChanged();
                }
            }
        }

        protected virtual void OnBrightnessChanged()
        {
        }

        protected virtual void OnContrastChanged()
        {
        }
    }
}
