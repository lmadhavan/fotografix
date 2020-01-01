using Fotografix.Adjustments;

namespace Fotografix.UI.Adjustments
{
    public sealed class BrightnessContrastAdjustmentViewModel : PropertyEditorViewModelBase<BrightnessContrastAdjustment>, IAdjustmentViewModel
    {
        public BrightnessContrastAdjustmentViewModel(BrightnessContrastAdjustment adjustment, ICommandService commandService) : base(adjustment, commandService)
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
