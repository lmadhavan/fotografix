using Fotografix.Drawing;
using Fotografix.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class CompositorPreviewTest : CompositionTestBase
    {
        private static readonly IDrawable BrushStroke = new BrushStroke(
            points: new Point[] { new Point(75, 100), new Point(250, 350) },
            size: 5,
            color: Color.White
        );

        private Layer background;

        [TestInitialize]
        public void Initialize()
        {
            this.background = Image.Layers[0];
        }

        [TestMethod]
        public async Task DrawingPreview_Background()
        {
            Layer foreground = await LoadLayerAsync("stars_small.jpg");
            Image.Layers.Add(foreground);

            background.SetDrawingPreview(BrushStroke);

            await AssertImageAsync("flowers_brush_stars_small.png");
        }

        [TestMethod]
        public async Task DrawingPreview_Foreground()
        {
            Layer foreground = await LoadLayerAsync("stars_small.jpg");
            Image.Layers.Add(foreground);

            foreground.SetDrawingPreview(BrushStroke);

            await AssertImageAsync("flowers_stars_small_brush.png");
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
