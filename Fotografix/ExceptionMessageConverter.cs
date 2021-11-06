using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Fotografix
{
    public sealed class ExceptionMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is AggregateException ae)
            {
                return string.Join('\n', ae.InnerExceptions.Select(ie => ie.Message));
            }

            if (value is Exception e)
            {
                return e.Message;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
