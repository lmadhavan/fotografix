namespace Fotografix.Uwp.Adjustments
{
    public sealed class AdjustmentSliderToolTipValueConverter : ValueConverter<double, string>
    {
        public override string Convert(double value)
        {
            if (value == 0)
            {
                return "0.00";
            }

            string s = string.Format("{0:F2}", value);
            return (value < 0) ? s : "+" + s;
        }
    }
}
