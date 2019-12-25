using System;
using Windows.UI.Xaml.Data;

namespace Fotografix.UI.Layers
{
    public sealed class OpacitySliderToolTipValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return string.Format("{0:P0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
