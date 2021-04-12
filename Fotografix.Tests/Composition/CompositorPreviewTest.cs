using Fotografix.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class CompositorPreviewTest : CompositionTestBase
    {
        private Layer background;

        [TestInitialize]
        public void Initialize()
        {
            this.background = Image.Layers[0];
        }

        [TestMethod]
        public async Task DrawingPreview_Background()
        {
            Layer foreground = await TestImages.LoadLayerAsync("stars_small.jpg");
            Image.Layers.Add(foreground);

            Bitmap bitmap = (Bitmap)background.Content;
            bitmap.SetDrawingPreview(TestDrawables.BrushStroke);

            await AssertImageAsync("flowers_brush_stars_small.png");
        }

        [TestMethod]
        public async Task DrawingPreview_Foreground()
        {
            Bitmap bitmap = await TestImages.LoadBitmapAsync("stars_small.jpg");
            Layer foreground = new Layer(bitmap);
            Image.Layers.Add(foreground);

            bitmap.SetDrawingPreview(TestDrawables.BrushStroke);

            await AssertImageAsync("flowers_stars_small_brush.png");
        }

        [TestMethod]
        public async Task DrawingPreview_Opacity()
        {
            Bitmap bitmap = await TestImages.LoadBitmapAsync("stars_small.jpg");
            Layer foreground = new Layer(bitmap);
            Image.Layers.Add(foreground);

            foreground.Opacity = 0.5f;
            bitmap.SetDrawingPreview(TestDrawables.BrushStroke);

            await AssertImageAsync("flowers_stars_small_opacity50_brush.png");
        }

        [TestMethod]
        public async Task DrawingPreview_Offset()
        {
            Bitmap bitmap = await TestImages.LoadBitmapAsync("stars_small.jpg");
            Layer foreground = new Layer(bitmap);
            Image.Layers.Add(foreground);

            bitmap.Position = new Point(50, 50);
            bitmap.SetDrawingPreview(TestDrawables.BrushStroke);

            await AssertImageAsync("flowers_stars_small_offset_brush.png");
        }

        [TestMethod]
        public async Task CropPreview()
        {
            Image.SetCropPreview(Rectangle.FromLTRB(100, 100, 200, 200));

            await AssertImageAsync("flowers_crop_preview.png");
        }

        [TestMethod]
        public async Task CropPreview_Scaled()
        {
            Viewport.ZoomFactor = 0.5f;
            Image.SetCropPreview(Rectangle.FromLTRB(100, 100, 200, 200));

            await AssertImageAsync("flowers_crop_preview_scale50.png");
        }
    }
}
