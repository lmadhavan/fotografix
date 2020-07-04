using Fotografix.Adjustments;
using System;
using Windows.UI.Xaml.Controls;

namespace Fotografix.UI.Adjustments
{
    public partial class AdjustmentTemplatesResourceDictionary
    {
        public AdjustmentTemplatesResourceDictionary()
        {
            InitializeComponent();

            // adjustment types
            RegisterAdjustment("Black & White", f => f.CreateBlackAndWhiteAdjustment());
            RegisterAdjustment("Brightness/Contrast", f => f.CreateBrightnessContrastAdjustment());
            RegisterAdjustment("Gradient Map", f => f.CreateGradientMapAdjustment());
            RegisterAdjustment("Hue/Saturation", f => f.CreateHueSaturationAdjustment());

            // adjustment property templates
            adjustmentTemplateSelector.SetTemplate<IBrightnessContrastAdjustment>(brightnessContrastTemplate);
            adjustmentTemplateSelector.SetTemplate<IGradientMapAdjustment>(gradientMapTemplate);
            adjustmentTemplateSelector.SetTemplate<IHueSaturationAdjustment>(hueSaturationTemplate);
        }

        private void RegisterAdjustment(string name, Func<IAdjustmentFactory, IAdjustment> createFunc)
        {
            newAdjustmentMenuFlyout.Items.Add(new MenuFlyoutItem()
            {
                Text = name,
                Tag = new AdjustmentLayerFactory(name, createFunc)
            });
        }
    }
}
