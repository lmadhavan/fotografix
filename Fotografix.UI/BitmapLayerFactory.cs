using Fotografix.Win2D;
using System.Drawing;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fotografix.UI
{
    public static class BitmapLayerFactory
    {
        private static IBitmapFactory bitmapFactory = new Win2DBitmapFactory();

        public static BitmapLayer CreateBitmapLayer(string name)
        {
            Bitmap bitmap = bitmapFactory.CreateBitmap(Size.Empty);
            return new BitmapLayer(bitmap) { Name = name };
        }

        public static BitmapLayer CreateBitmapLayer(int id)
        {
            return CreateBitmapLayer("Layer " + id);
        }

        public static async Task<BitmapLayer> LoadBitmapLayerAsync(StorageFile file)
        {
            Bitmap bitmap = await BitmapLoader.LoadBitmapAsync(file, bitmapFactory);
            return new BitmapLayer(bitmap) { Name = file.DisplayName };
        }
    }
}
