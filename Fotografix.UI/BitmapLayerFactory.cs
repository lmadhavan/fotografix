using System.Threading.Tasks;
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
            Bitmap bitmap = await BitmapLoader.LoadBitmapAsync(file);
            return new BitmapLayer(bitmap) { Name = file.DisplayName };
        }
    }
}
