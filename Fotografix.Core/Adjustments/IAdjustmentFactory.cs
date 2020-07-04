namespace Fotografix.Adjustments
{
    public interface IAdjustmentFactory
    {
        IBlackAndWhiteAdjustment CreateBlackAndWhiteAdjustment();
        IBrightnessContrastAdjustment CreateBrightnessContrastAdjustment();
        IGradientMapAdjustment CreateGradientMapAdjustment();
        IHueSaturationAdjustment CreateHueSaturationAdjustment();
    }
}
