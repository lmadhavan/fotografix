namespace Fotografix.Adjustments
{
    public sealed class AdjustmentFactory : IAdjustmentFactory
    {
        public IBlackAndWhiteAdjustment CreateBlackAndWhiteAdjustment()
        {
            return new BlackAndWhiteAdjustment();
        }

        public IBrightnessContrastAdjustment CreateBrightnessContrastAdjustment()
        {
            return new BrightnessContrastAdjustment();
        }

        public IGradientMapAdjustment CreateGradientMapAdjustment()
        {
            return new GradientMapAdjustment();
        }

        public IHueSaturationAdjustment CreateHueSaturationAdjustment()
        {
            return new HueSaturationAdjustment();
        }
    }
}
