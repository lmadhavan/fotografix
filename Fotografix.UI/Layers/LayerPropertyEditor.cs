using Fotografix.Editor.Adjustments;
using Fotografix.Editor.PropertyModel;
using Fotografix.UI.BlendModes;
using System.ComponentModel;

namespace Fotografix.UI.Layers
{
    public sealed class LayerPropertyEditor : PropertyEditor<Layer>
    {
        private static readonly BlendModeList BlendModeList = BlendModeList.Create();

        private IPropertyEditor adjustmentPropertyEditor;

        public LayerPropertyEditor(Layer layer, IPropertySetter propertySetter) : base(layer, propertySetter)
        {
            CreateAdjustmentViewModel();
        }

        public override void Dispose()
        {
            adjustmentPropertyEditor?.Dispose();
            base.Dispose();
        }

        public string Name
        {
            get => Target.Name;
            set => SetTargetProperty(value);
        }

        public BlendModeList AvailableBlendModes => BlendModeList;

        public BlendModeListItem BlendMode
        {
            get => BlendModeList[Target.BlendMode];
            set => SetTargetProperty(value.BlendMode);
        }

        public float Opacity
        {
            get => Target.Opacity;
            set => SetTargetProperty(value);
        }

        public IPropertyEditor AdjustmentPropertyEditor
        {
            get
            {
                return adjustmentPropertyEditor;
            }

            private set
            {
                adjustmentPropertyEditor?.Dispose();
                SetProperty(ref adjustmentPropertyEditor, value);
            }
        }

        protected override void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AdjustmentLayer.Adjustment))
            {
                CreateAdjustmentViewModel();
            }

            base.OnTargetPropertyChanged(sender, e);
        }

        private void CreateAdjustmentViewModel()
        {
            Target.Accept(new AdjustmentPropertyEditorFactoryVisitor(this));
        }

        private sealed class AdjustmentPropertyEditorFactoryVisitor : LayerVisitor
        {
            private readonly LayerPropertyEditor layerPropertyEditor;

            public AdjustmentPropertyEditorFactoryVisitor(LayerPropertyEditor layerViewModel)
            {
                this.layerPropertyEditor = layerViewModel;
            }

            public override void Visit(AdjustmentLayer layer)
            {
                layerPropertyEditor.AdjustmentPropertyEditor = layer.Adjustment.CreatePropertyEditor(layerPropertyEditor.PropertySetter);
            }
        }
    }
}
