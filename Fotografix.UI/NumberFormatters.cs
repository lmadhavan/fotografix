using Windows.Globalization.NumberFormatting;

namespace Fotografix.UI
{
    public static class NumberFormatters
    {
        public static INumberFormatter2 Dimension = new DecimalFormatter()
        {
            FractionDigits = 0,
            NumberRounder = new IncrementNumberRounder()
        };
    }
}
