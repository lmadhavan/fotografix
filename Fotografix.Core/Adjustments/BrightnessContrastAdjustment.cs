using System;

namespace Fotografix.Adjustments
{
    public sealed class BrightnessContrastAdjustment : Adjustment
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

                SetProperty(ref brightness, value);
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

                SetProperty(ref contrast, value);
            }
        }
    }
}
