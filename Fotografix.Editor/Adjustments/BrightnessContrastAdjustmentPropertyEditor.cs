using Fotografix.Adjustments;
using Fotografix.Editor.PropertyModel;

namespace Fotografix.Editor.Adjustments
{
    public sealed class BrightnessContrastAdjustmentPropertyEditor : PropertyEditor<BrightnessContrastAdjustment>
    {
        public BrightnessContrastAdjustmentPropertyEditor(BrightnessContrastAdjustment adjustment, IPropertySetter propertySetter) : base(adjustment, propertySetter)
        {
        }

        public float Brightness
        {
            get => Target.Brightness;
            set => SetTargetProperty(value);
        }

        public float Contrast
        {
            get => Target.Contrast;
            set => SetTargetProperty(value);
        }
    }
}
