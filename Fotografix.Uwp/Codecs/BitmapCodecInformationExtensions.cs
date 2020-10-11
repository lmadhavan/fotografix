using Fotografix.IO;
using Windows.Graphics.Imaging;

namespace Fotografix.Uwp.Codecs
{
    internal static class BitmapCodecInformationExtensions
    {
        public static FileFormat ToFileFormat(this BitmapCodecInformation bci, string suffixToStrip)
        {
            string name = bci.FriendlyName;
            if (name.EndsWith(suffixToStrip))
            {
                name = name.Substring(0, name.Length - suffixToStrip.Length);
            }

            return new FileFormat(name, bci.FileExtensions);
        }
    }
}
