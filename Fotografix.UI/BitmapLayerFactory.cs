using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.UI
{
    public static class BitmapLayerFactory
    {
        public static BitmapLayer CreateBitmapLayer(string name, IBitmapFactory bitmapFactory)
        {
            Bitmap bitmap = bitmapFactory.CreateBitmap(Size.Empty);
            return new BitmapLayer(bitmap) { Name = name };
        }

        public static BitmapLayer CreateBitmapLayer(int id, IBitmapFactory bitmapFactory)
        {
            return CreateBitmapLayer("Layer " + id, bitmapFactory);
        }

        public static async Task<BitmapLayer> LoadBitmapLayerAsync(StorageFile file, IBitmapFactory bitmapFactory)
        {
            Bitmap bitmap = await BitmapCodec.LoadBitmapAsync(file, bitmapFactory);
            return new BitmapLayer(bitmap) { Name = file.DisplayName };
        }
    }
}
