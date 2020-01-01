using Fotografix.Adjustments;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI.Adjustments
{
    public partial class AdjustmentTemplatesResourceDictionary
    {
        public AdjustmentTemplatesResourceDictionary()
        {
            InitializeComponent();

            // adjustment types
            RegisterAdjustment<BlackAndWhiteAdjustment>("Black & White");
            RegisterAdjustment<BrightnessContrastAdjustment>("Brightness/Contrast");
            RegisterAdjustment<GradientMapAdjustment>("Gradient Map");
            RegisterAdjustment<HueSaturationAdjustment>("Hue/Saturation");

            // adjustment property templates
            adjustmentTemplateSelector.SetTemplate<BrightnessContrastAdjustmentViewModel>(brightnessContrastTemplate);
            adjustmentTemplateSelector.SetTemplate<GradientMapAdjustmentViewModel>(gradientMapTemplate);
            adjustmentTemplateSelector.SetTemplate<HueSaturationAdjustmentViewModel>(hueSaturationTemplate);
        }

        private void RegisterAdjustment<T>(string name) where T : Adjustment, new()
        {
            newAdjustmentMenuFlyout.Items.Add(new MenuFlyoutItem()
            {
                Text = name,
                Tag = new AdjustmentLayerFactory<T>(name)
            });
        }
    }
}
