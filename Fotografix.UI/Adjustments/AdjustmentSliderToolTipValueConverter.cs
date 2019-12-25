using System;
using Windows.UI.Xaml.Data;

namespace Fotografix.UI.Adjustments
{
    public sealed class AdjustmentSliderToolTipValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double d = (double)value;

            if (d == 0)
            {
                return "0.00";
            }

            string s = string.Format("{0:F2}", d);
            return (d < 0) ? s : "+" + s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
