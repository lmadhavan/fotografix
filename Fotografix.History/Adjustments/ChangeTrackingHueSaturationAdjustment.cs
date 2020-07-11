using Fotografix.Adjustments;

namespace Fotografix.History.Adjustments
{
    public sealed class ChangeTrackingHueSaturationAdjustment : PropertyChangeTrackingBase<IHueSaturationAdjustment>, IHueSaturationAdjustment
    {
        public ChangeTrackingHueSaturationAdjustment(IHueSaturationAdjustment adjustment, IChangeTracker changeTracker)
            : base(adjustment, changeTracker)
        {
        }

        public float Hue
        {
            get
            {
                return Target.Hue;
            }

            set
            {
                AddPropertyChange(Target.Hue, value);
                Target.Hue = value;
            }
        }

        public float Saturation
        {
            get
            {
                return Target.Saturation;
            }

            set
            {
                AddPropertyChange(Target.Saturation, value);
                Target.Saturation = value;
            }
        }

        public float Lightness
        {
            get
            {
                return Target.Lightness;
            }

            set
            {
                AddPropertyChange(Target.Lightness, value);
                Target.Lightness = value;
            }
        }
    }
}
