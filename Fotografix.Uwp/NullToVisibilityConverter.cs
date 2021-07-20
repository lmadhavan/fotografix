using Windows.UI.Xaml;

namespace Fotografix.Uwp
{
    public sealed class NullToVisibilityConverter : ValueConverter<object, Visibility>
    {
        public override Visibility Convert(object value)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
