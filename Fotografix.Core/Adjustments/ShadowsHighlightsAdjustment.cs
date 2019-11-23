using System;

namespace Fotografix.Adjustments
{
    public sealed class ShadowsHighlightsAdjustment : Adjustment
    {
        private float shadows;
        private float highlights;
        private float clarity;

        public float Shadows
        {
            get
            {
                return shadows;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref shadows, value);
            }
        }

        public float Highlights
        {
            get
            {
                return highlights;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref highlights, value);
            }
        }

        public float Clarity
        {
            get
            {
                return clarity;
            }

            set
            {
                if (value < -1 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref clarity, value);
            }
        }
    }
}