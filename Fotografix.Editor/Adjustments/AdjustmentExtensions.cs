using Fotografix.Adjustments;
using Fotografix.Editor.PropertyModel;

namespace Fotografix.Editor.Adjustments
{
    public static class AdjustmentExtensions
    {
        public static IPropertyEditor CreatePropertyEditor(this Adjustment adjustment, IPropertySetter propertySetter)
        {
            FactoryVisitor visitor = new FactoryVisitor(propertySetter);
            adjustment.Accept(visitor);
            return visitor.PropertyEditor;
        }

        private sealed class FactoryVisitor : AdjustmentVisitor
        {
            private readonly IPropertySetter propertySetter;

            public FactoryVisitor(IPropertySetter propertySetter)
            {
                this.propertySetter = propertySetter;
            }

            public IPropertyEditor PropertyEditor { get; private set; }

            public override void Visit(BrightnessContrastAdjustment adjustment)
            {
                this.PropertyEditor = new BrightnessContrastAdjustmentPropertyEditor(adjustment, propertySetter);
            }

            public override void Visit(GradientMapAdjustment adjustment)
            {
                this.PropertyEditor = new GradientMapAdjustmentPropertyEditor(adjustment, propertySetter);
            }

            public override void Visit(HueSaturationAdjustment adjustment)
            {
                this.PropertyEditor = new HueSaturationAdjustmentPropertyEditor(adjustment, propertySetter);
            }
        }
    }
}
