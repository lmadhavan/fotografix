using Fotografix.Adjustments;
using Fotografix.Editor.PropertyModel;

namespace Fotografix.Editor.Adjustments
{
    public sealed class HueSaturationAdjustmentPropertyEditor : PropertyEditor<HueSaturationAdjustment>
    {
        public HueSaturationAdjustmentPropertyEditor(HueSaturationAdjustment adjustment, IPropertySetter propertySetter) : base(adjustment, propertySetter)
        {
        }

        public float Hue
        {
            get => Target.Hue;
            set => SetTargetProperty(value);
        }

        public float Saturation
        {
            get => Target.Saturation;
            set => SetTargetProperty(value);
        }

        public float Lightness
        {
            get => Target.Saturation;
            set => SetTargetProperty(value);
        }
    }
}
