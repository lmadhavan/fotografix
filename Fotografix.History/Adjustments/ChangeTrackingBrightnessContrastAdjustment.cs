using Fotografix.Adjustments;

namespace Fotografix.History.Adjustments
{
    public sealed class ChangeTrackingBrightnessContrastAdjustment : PropertyChangeTrackingBase<IBrightnessContrastAdjustment>, IBrightnessContrastAdjustment
    {
        public ChangeTrackingBrightnessContrastAdjustment(IBrightnessContrastAdjustment adjustment, IChangeTracker changeTracker)
            : base(adjustment, changeTracker)
        {
        }

        public float Brightness
        {
            get
            {
                return Target.Brightness;
            }

            set
            {
                AddPropertyChange(Target.Brightness, value);
                Target.Brightness = value;
            }
        }

        public float Contrast
        {
            get
            {
                return Target.Contrast;
            }

            set
            {
                AddPropertyChange(Target.Contrast, value);
                Target.Contrast = value;
            }
        }
    }
}
