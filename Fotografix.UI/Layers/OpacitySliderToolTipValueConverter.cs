namespace Fotografix.UI.Layers
{
    public sealed class OpacitySliderToolTipValueConverter : ValueConverter<double, string>
    {
        public override string Convert(double value)
        {
            return string.Format("{0:P0}", value);
        }
    }
}
