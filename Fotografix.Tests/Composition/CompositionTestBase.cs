using Fotografix.Drawing;
using Fotografix.UI;
using Fotografix.Win2D;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    public abstract class CompositionTestBase
    {
        private static readonly IDrawingContextFactory DrawingContextFactory = new Win2DDrawingContextFactory();
     
        protected async Task<Image> LoadImageAsync(string filename)
        {
            var layer = await LoadLayerAsync(filename);
            return new Image(layer);
        }

        protected async Task<BitmapLayer> LoadLayerAsync(string filename)
        {
            var file = await TestImages.GetFileAsync(filename);
            return await BitmapLayerFactory.LoadBitmapLayerAsync(file);
        }

        protected async Task AssertImageAsync(string fileWithExpectedOutput, Image image)
        {
            Bitmap bitmap = image.ToBitmap(DrawingContextFactory);
            await AssertImage.IsEquivalentAsync(fileWithExpectedOutput, bitmap);
        }

        protected async Task CaptureToTempFolderAsync(Image image, string filename)
        {
            Bitmap bitmap = image.ToBitmap(DrawingContextFactory);
            await bitmap.CaptureToTempFolderAsync(filename);
        }
    }
}