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

            background.SetDrawingPreview(TestDrawables.BrushStroke);

            await AssertImageAsync("flowers_brush_stars_small.png");
        }

        [TestMethod]
        public async Task DrawingPreview_Foreground()
        {
            Layer foreground = await TestImages.LoadLayerAsync("stars_small.jpg");
            Image.Layers.Add(foreground);

            foreground.SetDrawingPreview(TestDrawables.BrushStroke);

            await AssertImageAsync("flowers_stars_small_brush.png");
        }

        [TestMethod]
        public async Task DrawingPreview_Opacity()
        {
            Layer foreground = await TestImages.LoadLayerAsync("stars_small.jpg");
            Image.Layers.Add(foreground);

            foreground.Opacity = 0.5f;
            foreground.SetDrawingPreview(TestDrawables.BrushStroke);

            await AssertImageAsync("flowers_stars_small_opacity50_brush.png");
        }

        [TestMethod]
        public async Task DrawingPreview_Offset()
        {
            Layer foreground = await TestImages.LoadLayerAsync("stars_small.jpg");
            Image.Layers.Add(foreground);

            foreground.Position = new Point(50, 50);
            foreground.SetDrawingPreview(TestDrawables.BrushStroke);

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
