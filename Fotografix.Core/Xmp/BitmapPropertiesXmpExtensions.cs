using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;

namespace Fotografix.Xmp
{
    public static class BitmapPropertiesXmpExtensions
    {
        public static async Task<BitmapTypedValue> GetXmpPropertyAsync(this IBitmapPropertiesView props, XmpProperty property)
        {
            var key = MetadataKey(property);
            var metadata = await props.GetPropertiesAsync(new string[] { key });
            return metadata[key];
        }

        public static Task SetXmpPropertyAsync(this BitmapProperties props, XmpProperty property, string value)
        {
            return SetXmpPropertyAsync(props, property, new BitmapTypedValue(value, PropertyType.String));
        }

        public static Task SetXmpPropertyAsync(this BitmapProperties props, XmpProperty property, BitmapTypedValue value)
        {
            var metadata = new Dictionary<string, BitmapTypedValue>
            {
                [MetadataKey(property)] = value
            };

            return props.SetPropertiesAsync(metadata).AsTask();
        }

        private static string MetadataKey(XmpProperty property)
        {
            return $"/xmp/{{wstr={property.NamespaceUri}}}:{property.Name}";
        }
    }
}
