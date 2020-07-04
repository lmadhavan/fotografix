namespace Fotografix.Adjustments
{
    public interface IHueSaturationAdjustment : IAdjustment
    {
        float Hue { get; set; }
        float Saturation { get; set; }
        float Lightness { get; set; }
    }
}