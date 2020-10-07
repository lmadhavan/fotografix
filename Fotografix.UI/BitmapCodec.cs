using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Fotografix.UI
{
    public static class BitmapCodec
    {
        public static async Task<Bitmap> LoadBitmapAsync(StorageFile file)
        {
            using (var stream = await file.OpenReadAsync())
            {
                return await WindowsImageDecoder.ReadBitmapAsync(stream);
            }
        }

        public static async Task SaveBitmapAsync(StorageFile file, Bitmap bitmap)
        {
            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                Guid encoderId = file.Name.ToLower().EndsWith("png") ? BitmapEncoder.PngEncoderId : BitmapEncoder.JpegEncoderId;
                await WindowsImageEncoder.WriteBitmapAsync(encoderId, stream, bitmap);
            }
        }
    }
}
