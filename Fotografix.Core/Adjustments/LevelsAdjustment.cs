using System;

namespace Fotografix.Adjustments
{
    public sealed class LevelsAdjustment : Adjustment
    {
        private float inputBlackPoint = 0;
        private float inputWhitePoint = 1;
        private float inputGamma = 1;
        private float outputBlackPoint = 0;
        private float outputWhitePoint = 1;

        public float InputBlackPoint
        {
            get
            {
                return inputBlackPoint;
            }

            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref inputBlackPoint, value);
            }
        }

        public float InputWhitePoint
        {
            get
            {
                return inputWhitePoint;
            }

            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref inputWhitePoint, value);
            }
        }

        public float InputGamma
        {
            get
            {
                return inputGamma;
            }

            set
            {
                SetProperty(ref inputGamma, value);
            }
        }

        public float OutputBlackPoint
        {
            get
            {
                return outputBlackPoint;
            }

            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref outputBlackPoint, value);
            }
        }

        public float OutputWhitePoint
        {
            get
            {
                return outputWhitePoint;
            }

            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                SetProperty(ref outputWhitePoint, value);
            }
        }
    }
}
