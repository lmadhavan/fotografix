using Fotografix.Win2D;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    public abstract class CompositionTestBase
    {
        private static readonly IImageRenderer Renderer = new Win2DImageRenderer();
     
        protected async Task<Image> LoadImageAsync(string filename)
        {
            var layer = await LoadLayerAsync(filename);
            return new Image(layer);
        }

        protected async Task<BitmapLayer> LoadLayerAsync(string filename)
        {
            Bitmap bitmap = await TestImages.LoadBitmapAsync(filename);
            return new BitmapLayer(bitmap);
        }

        protected async Task AssertImageAsync(string fileWithExpectedOutput, Image image)
        {
            Bitmap bitmap = Renderer.Render(image);
            await AssertImage.IsEquivalentAsync(fileWithExpectedOutput, bitmap);
        }

        protected async Task CaptureToTempFolderAsync(Image image, string filename)
        {
            Bitmap bitmap = Renderer.Render(image);
            await bitmap.CaptureToTempFolderAsync(filename);
        }
    }
}