using Fotografix.Adjustments;
using Fotografix.Editor.PropertyModel;

namespace Fotografix.Editor.Adjustments
{
    public sealed class LevelsAdjustmentPropertyEditor : PropertyEditor<LevelsAdjustment>
    {
        public LevelsAdjustmentPropertyEditor(LevelsAdjustment adjustment, IPropertySetter propertySetter) : base(adjustment, propertySetter)
        {
        }

        public float InputBlackPoint
        {
            get => Target.InputBlackPoint;
            set => SetTargetProperty(value);
        }

        public float InputWhitePoint
        {
            get => Target.InputWhitePoint;
            set => SetTargetProperty(value);
        }

        public float InputGamma
        {
            get => Target.InputGamma;
            set => SetTargetProperty(value);
        }

        public float OutputBlackPoint
        {
            get => Target.OutputBlackPoint;
            set => SetTargetProperty(value);
        }

        public float OutputWhitePoint
        {
            get => Target.OutputWhitePoint;
            set => SetTargetProperty(value);
        }
    }
}
