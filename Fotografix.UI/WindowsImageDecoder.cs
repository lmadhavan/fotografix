using Fotografix.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Fotografix.UI
{
    public sealed class WindowsImageDecoder : IImageDecoder
    {
        public WindowsImageDecoder()
        {
            this.SupportedFileFormats = BitmapDecoder.GetDecoderInformationEnumerator()
                                                     .Select(bci => bci.ToFileFormat(suffixToStrip: " Decoder"));
        }

        public IEnumerable<FileFormat> SupportedFileFormats { get; }

        public async Task<Image> ReadImageAsync(IFile file)
        {
            using (Stream stream = await file.OpenReadAsync())
            {
                Bitmap bitmap = await ReadBitmapAsync(stream.AsRandomAccessStream());
                BitmapLayer layer = new BitmapLayer(bitmap) { Name = file.Name };
                return new Image(layer);
            }
        }

        public static async Task<Bitmap> ReadBitmapAsync(IRandomAccessStream stream)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

            var pixelDataProvider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                    BitmapAlphaMode.Premultiplied,
                                                                    new BitmapTransform(),
                                                                    ExifOrientationMode.RespectExifOrientation,
                                                                    ColorManagementMode.ColorManageToSRgb);

            Size size = new Size((int)decoder.OrientedPixelWidth, (int)decoder.OrientedPixelHeight);
            return new Bitmap(size, pixelDataProvider.DetachPixelData());
        }
    }
}
