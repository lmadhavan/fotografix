using Fotografix.UI;
using Fotografix.Win2D;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    public abstract class CompositionTestBase
    {
        private static readonly IBitmapFactory BitmapFactory = new Win2DBitmapFactory();
     
        protected async Task<Image> LoadImageAsync(string filename)
        {
            var layer = await LoadLayerAsync(filename);
            return new Image(layer);
        }

        protected async Task<BitmapLayer> LoadLayerAsync(string filename)
        {
            var file = await TestImages.GetFileAsync(filename);
            return await BitmapLayerFactory.LoadBitmapLayerAsync(file, BitmapFactory);
        }

        protected async Task AssertImageAsync(string fileWithExpectedOutput, Image image)
        {
            using (Bitmap bitmap = image.ToBitmap(BitmapFactory))
            {
                await AssertImage.IsEquivalentAsync(fileWithExpectedOutput, bitmap);
            }
        }

        protected async Task CaptureToTempFolderAsync(Image image, string filename)
        {
            using (Bitmap bitmap = image.ToBitmap(BitmapFactory))
            {
                await bitmap.CaptureToTempFolderAsync(filename);
            }
        }
    }
}