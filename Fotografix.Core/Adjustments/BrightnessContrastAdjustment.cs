namespace Fotografix.Adjustments
{
    public sealed class BrightnessContrastAdjustment : Adjustment
    {
        private float brightness;
        private float contrast;

        public float Brightness
        {
            get => brightness;
            set => SetProperty(ref brightness, value);
        }

        public float Contrast
        {
            get => contrast;
            set => SetProperty(ref contrast, value);
        }
    }
}
