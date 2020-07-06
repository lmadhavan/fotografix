using Fotografix.Adjustments;
using Fotografix.UI;
using Fotografix.Win2D;
using Fotografix.Win2D.Adjustments;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    public abstract class CompositionTestBase
    {
        protected IAdjustmentFactory AdjustmentFactory { get; } = new Win2DAdjustmentFactory();

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
            using (Win2DCompositor compositor = new Win2DCompositor(actualImage))
            {
                await AssertImage.IsEquivalentAsync(fileWithExpectedOutput, compositor);
            }
        }
    }
}