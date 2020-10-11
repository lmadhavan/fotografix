using System;
using Windows.UI.Xaml.Data;

namespace Fotografix.Uwp
{
    /// <summary>
    /// Converts between <see cref="System.Drawing.Color"/> and <see cref="Windows.UI.Color"/>.
    /// </summary>
    public sealed class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            System.Drawing.Color color = (System.Drawing.Color)value;
            return Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Windows.UI.Color color = (Windows.UI.Color)value;
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
