using Fotografix.Adjustments;
using System.Drawing;

namespace Fotografix.UI.Adjustments
{
    public sealed class GradientMapAdjustmentViewModel : PropertyEditorViewModelBase<GradientMapAdjustment>, IAdjustmentViewModel
    {
        public GradientMapAdjustmentViewModel(GradientMapAdjustment adjustment, ICommandService commandService) : base(adjustment, commandService)
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
