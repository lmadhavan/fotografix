using Fotografix.Adjustments;

namespace Fotografix.Win2D.Adjustments
{
    public sealed class Win2DAdjustmentFactory : IAdjustmentFactory
    {
        public IBlackAndWhiteAdjustment CreateBlackAndWhiteAdjustment()
        {
            return new Win2DBlackAndWhiteAdjustment();
        }

        public IBrightnessContrastAdjustment CreateBrightnessContrastAdjustment()
        {
            return new Win2DBrightnessContrastAdjustment();
        }

        public IGradientMapAdjustment CreateGradientMapAdjustment()
        {
            return new Win2DGradientMapAdjustment();
        }

        public IHueSaturationAdjustment CreateHueSaturationAdjustment()
        {
            return new Win2DHueSaturationAdjustment();
        }
    }
}
