using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Fotografix.Uwp
{
    public sealed class GeometryResourceConverter : ValueConverter<string, Geometry>
    {
        public string KeySuffix { get; set; } = "";

        public override Geometry Convert(string value)
        {
            string key = value + KeySuffix;

            string path = (string)Application.Current.Resources[key];
            return (Geometry)XamlBindingHelper.ConvertValue(typeof(Geometry), path);
        }
    }
}
