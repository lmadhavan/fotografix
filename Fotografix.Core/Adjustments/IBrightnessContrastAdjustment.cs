namespace Fotografix.Adjustments
{
    public interface IBrightnessContrastAdjustment : IAdjustment
    {
        float Brightness { get; set; }
        float Contrast { get; set; }
    }
}