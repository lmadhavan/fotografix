using System;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Fotografix.UI
{
    public static class BitmapLayerFactory
    {
        public static BitmapLayer CreateBitmapLayer(string name)
        {
            return new BitmapLayer(Bitmap.Empty) { Name = name };
        }

        public static BitmapLayer CreateBitmapLayer(int id)
        {
            return CreateBitmapLayer("Layer " + id);
        }

        public static async Task<BitmapLayer> LoadBitmapLayerAsync(StorageFile file)
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
                Bitmap bitmap = new Bitmap(size, pixelDataProvider.DetachPixelData());

                return new BitmapLayer(bitmap) { Name = file.DisplayName };
            }
        }
    }
}
