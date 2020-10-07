using Fotografix.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Fotografix.UI
{
    public sealed class WindowsImageEncoder : IImageEncoder
    {
        private readonly IImageRenderer renderer;
        private readonly Dictionary<FileFormat, Guid> formatToGuid = new Dictionary<FileFormat, Guid>();

        public WindowsImageEncoder(IImageRenderer renderer)
        {
            this.renderer = renderer;
            this.formatToGuid = BitmapEncoder.GetEncoderInformationEnumerator().ToDictionary(
                                                        bci => bci.ToFileFormat(suffixToStrip: " Encoder"),
                                                        bci => bci.CodecId);
        }

        public IEnumerable<FileFormat> SupportedFileFormats => formatToGuid.Keys;

        public async Task WriteImageAsync(Image image, IFile file, FileFormat fileFormat)
        {
            Guid encoderId = formatToGuid[fileFormat];
            Bitmap bitmap = renderer.Render(image);

            using (Stream stream = await file.OpenWriteAsync())
            {
                await WriteBitmapAsync(encoderId, stream.AsRandomAccessStream(), bitmap);
            }
        }

        public static async Task WriteBitmapAsync(Guid encoderId, IRandomAccessStream stream, Bitmap bitmap)
        {
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(encoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                 BitmapAlphaMode.Premultiplied,
                                 (uint)bitmap.Size.Width,
                                 (uint)bitmap.Size.Height,
                                 96,
                                 96,
                                 bitmap.Pixels);
            await encoder.FlushAsync();
        }
    }
}
