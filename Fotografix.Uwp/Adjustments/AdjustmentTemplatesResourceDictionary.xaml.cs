using Fotografix.Adjustments;
using Windows.UI.Xaml.Controls;

namespace Fotografix.Uwp.Adjustments
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
            RegisterAdjustment<LevelsAdjustment>("Levels");

            // adjustment property templates
            adjustmentTemplateSelector.SetTemplate<BrightnessContrastAdjustment>(brightnessContrastTemplate);
            adjustmentTemplateSelector.SetTemplate<GradientMapAdjustment>(gradientMapTemplate);
            adjustmentTemplateSelector.SetTemplate<HueSaturationAdjustment>(hueSaturationTemplate);
            adjustmentTemplateSelector.SetTemplate<LevelsAdjustment>(levelsTemplate);
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
