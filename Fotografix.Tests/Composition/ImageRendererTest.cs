using Fotografix.Editor;
using Fotografix.Win2D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class ImageRendererTest
    {
        private IImageRenderer renderer;

        [TestInitialize]
        public void Initialize()
        {
            this.renderer = new Win2DImageRenderer();
        }

        [TestMethod]
        public async Task DoesNotRenderPreviews()
        {
            Image image = await TestImages.LoadImageAsync("flowers.jpg");
            BitmapLayer layer = await TestImages.LoadLayerAsync("stars_small.jpg");

            image.Layers.Add(layer);
            image.SetCropPreview(new Rectangle(0, 0, 100, 100));
            layer.SetDrawingPreview(TestDrawables.BrushStroke);

            Bitmap bitmap = renderer.Render(image);
            await AssertImage.IsEquivalentAsync("flowers_stars_small.png", bitmap);
        }
    }
}
