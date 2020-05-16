using Fotografix.UI;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    public abstract class CompositionTestBase
    {
        protected async Task<Image> LoadImageAsync(string filename)
        {
            var layer = await LoadLayerAsync(filename);

            Image image = new Image(layer.Bitmap.Size);
            image.Layers.Add(layer);
            return image;
        }

        protected async Task<BitmapLayer> LoadLayerAsync(string filename)
        {
            var file = await TestImages.GetFileAsync(filename);
            return await BitmapLayerFactory.LoadBitmapLayerAsync(file);
        }

        protected async Task AssertImageAsync(string fileWithExpectedOutput, Image actualImage)
        {
            await AssertImage.IsEquivalentAsync(fileWithExpectedOutput, actualImage);
        }
    }
}