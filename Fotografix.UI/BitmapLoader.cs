using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Fotografix.UI
{
    public static class BitmapLoader
    {
        public static async Task<Bitmap> LoadBitmapAsync(StorageFile file, IBitmapFactory bitmapFactory)
        {
            using (var stream = await file.OpenReadAsync())
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                var pixelDataProvider = await decoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                        BitmapAlphaMode.Premultiplied,
                                                                        new BitmapTransform(),
                                                                        ExifOrientationMode.RespectExifOrientation,
                                                                        ColorManagementMode.ColorManageToSRgb);

                Size size = new Size((int)decoder.OrientedPixelWidth, (int)decoder.OrientedPixelHeight);
                Bitmap bitmap = bitmapFactory.CreateBitmap(size);
                bitmap.SetPixelBytes(pixelDataProvider.DetachPixelData());
                return bitmap;
            }
        }
    }
}
