using Fotografix.Adjustments;
using System.Drawing;

namespace Fotografix.History.Adjustments
{
    public sealed class ChangeTrackingGradientMapAdjustment : ChangeTrackingAdjustment<IGradientMapAdjustment>, IGradientMapAdjustment
    {
        public ChangeTrackingGradientMapAdjustment(IGradientMapAdjustment adjustment, IChangeTracker changeTracker)
            : base(adjustment, changeTracker)
        {
        }

        public Color Shadows
        {
            get
            {
                return Target.Shadows;
            }

            set
            {
                AddPropertyChange(Target.Shadows, value);
                Target.Shadows = value;
            }
        }

        public Color Highlights
        {
            get
            {
                return Target.Highlights;
            }

            set
            {
                AddPropertyChange(Target.Highlights, value);
                Target.Highlights = value;
            }
        }
    }
}
