using Fotografix.Adjustments;
using Fotografix.Editor.PropertyModel;
using System.Drawing;

namespace Fotografix.Editor.Adjustments
{
    public sealed class GradientMapAdjustmentPropertyEditor : PropertyEditor<GradientMapAdjustment>
    {
        public GradientMapAdjustmentPropertyEditor(GradientMapAdjustment adjustment, IPropertySetter propertySetter) : base(adjustment, propertySetter)
        {
        }

        public Color Shadows
        {
            get => Target.Shadows;
            set => SetTargetProperty(value);
        }

        public Color Highlights
        {
            get => Target.Highlights;
            set => SetTargetProperty(value);
        }
    }
}
