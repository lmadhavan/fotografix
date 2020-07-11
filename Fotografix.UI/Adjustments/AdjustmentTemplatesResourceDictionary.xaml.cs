using Fotografix.Adjustments;
using Fotografix.Editor.Adjustments;
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
            adjustmentTemplateSelector.SetTemplate<BrightnessContrastAdjustmentPropertyEditor>(brightnessContrastTemplate);
            adjustmentTemplateSelector.SetTemplate<GradientMapAdjustmentPropertyEditor>(gradientMapTemplate);
            adjustmentTemplateSelector.SetTemplate<HueSaturationAdjustmentPropertyEditor>(hueSaturationTemplate);
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
