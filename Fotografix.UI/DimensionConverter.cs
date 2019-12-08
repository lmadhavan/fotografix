using System;
using Windows.UI.Xaml.Data;

namespace Fotografix.UI
{
    public sealed class DimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (int.TryParse((string)value, out int result) && result > 0)
            {
                return result;
            }

            return 1;
        }
    }
}
