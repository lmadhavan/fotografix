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
            this.Image = await TestImages.LoadImageAsync("flowers.jpg");
            this.Viewport = new Viewport(Image.Size);
            this.compositor = new Win2DCompositor(Image, Viewport, new Win2DCompositorSettings { InteractiveMode = true });
        }

        [TestCleanup]
        public void DisposeCompositor()
        {
            compositor.Dispose();
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