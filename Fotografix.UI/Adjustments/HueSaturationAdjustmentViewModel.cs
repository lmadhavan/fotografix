using Fotografix.Adjustments;

namespace Fotografix.UI.Adjustments
{
    public sealed class HueSaturationAdjustmentViewModel : PropertyEditorViewModelBase<HueSaturationAdjustment>, IAdjustmentViewModel
    {
        public HueSaturationAdjustmentViewModel(HueSaturationAdjustment adjustment, ICommandService commandService) : base(adjustment, commandService)
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
