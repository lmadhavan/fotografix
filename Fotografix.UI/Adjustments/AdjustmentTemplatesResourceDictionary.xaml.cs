using Fotografix.Adjustments;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI.Adjustments
{
    public partial class AdjustmentTemplatesResourceDictionary
    {
        public AdjustmentTemplatesResourceDictionary()
        {
            InitializeComponent();

            RegisterAdjustment<BlackAndWhiteAdjustment>("Black & White", blackAndWhiteTemplate);
            RegisterAdjustment<ShadowsHighlightsAdjustment>("Shadows/Highlights", shadowsHighlightsTemplate);
            RegisterAdjustment<HueSaturationAdjustment>("Hue/Saturation", hueSaturationTemplate);
            RegisterAdjustment<GradientMapAdjustment>("Gradient Map", gradientMapTemplate);
        }

        private void RegisterAdjustment<T>(string name, DataTemplate dataTemplate) where T : Adjustment, new()
        {
            adjustmentTemplateSelector.SetTemplate<T>(dataTemplate);

            newAdjustmentMenuFlyout.Items.Add(new MenuFlyoutItem()
            {
                Text = name,
                Tag = new AdjustmentLayerFactory<T>(name)
            });
        }
    }
}
