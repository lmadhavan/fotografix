using Fotografix.Adjustments;
using Fotografix.Editor.PropertyModel;

namespace Fotografix.Editor.Layers
{
    public sealed class LayerPropertyEditor : PropertyEditor<Layer>
    {
        public LayerPropertyEditor(Layer layer, IPropertySetter propertySetter) : base(layer, propertySetter)
        {
            if (layer is AdjustmentLayer a)
            {
                this.Adjustment = a.Adjustment;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public string Name
        {
            get => Target.Name;
            set => SetTargetProperty(value);
        }

        public BlendMode BlendMode
        {
            get => Target.BlendMode;
            set => SetTargetProperty(value);
        }

        public float Opacity
        {
            get => Target.Opacity;
            set => SetTargetProperty(value);
        }

        public IAdjustment Adjustment { get; }
    }
}
