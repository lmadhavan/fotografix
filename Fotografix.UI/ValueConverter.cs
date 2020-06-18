using System;
using Windows.UI.Xaml.Data;

namespace Fotografix.UI
{
    public abstract class ValueConverter<TModel, TDisplay> : IValueConverter
    {
        public abstract TDisplay Convert(TModel value);

        public virtual TModel ConvertBack(TDisplay value)
        {
            throw new NotImplementedException();
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            return Convert((TModel)value);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return ConvertBack((TDisplay)value);
        }
    }
}
