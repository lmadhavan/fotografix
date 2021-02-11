using Fotografix.Editor;
using Fotografix.Win2D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    public abstract class CompositionTestBase
    {
        private Win2DCompositor compositor;

        protected Image Image { get; private set; }
        protected Viewport Viewport { get; private set; }

        [TestInitialize]
        public async Task InitializeImageAndCompositor()
        {
            this.Image = await LoadImageAsync("flowers.jpg");
            this.Viewport = new Viewport(Image.Size);
            this.compositor = new Win2DCompositor(Image, Viewport, 0);
        }

        [TestCleanup]
        public void DisposeCompositor()
        {
            compositor.Dispose();
        }

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

        protected async Task AssertImageAsync(string fileWithExpectedOutput)
        {
            Bitmap bitmap = compositor.ToBitmap();
            await AssertImage.IsEquivalentAsync(fileWithExpectedOutput, bitmap);
        }

        protected async Task CaptureToTempFolderAsync(string filename)
        {
            Bitmap bitmap = compositor.ToBitmap();
            await bitmap.CaptureToTempFolderAsync(filename);
        }
    }
}