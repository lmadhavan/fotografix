using Fotografix.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class ImageAdjustmentsTest : CompositionTestBase
    {
        private Image image;

        [TestInitialize]
        public async Task Initialize()
        {
            this.image = await LoadImageAsync("flowers.jpg");
        }

        [TestMethod]
        public async Task BlackAndWhiteAdjustment()
        {
            AddAdjustment(new BlackAndWhiteAdjustment());

            await AssertImageAsync("flowers_bw.png", image);
        }

        [TestMethod]
        public async Task ShadowsHighlightsAdjustment()
        {
            AddAdjustment(new ShadowsHighlightsAdjustment()
            {
                Shadows = 0.25f,
                Highlights = -0.25f,
                Clarity = 0.5f
            });

            await AssertImageAsync("flowers_sh.png", image);
        }

        [TestMethod]
        public async Task HueSaturationAdjustment()
        {
            AddAdjustment(new HueSaturationAdjustment()
            {
                Hue = 0.5f,
                Saturation = 0.25f,
                Lightness = 0.25f
            });

            await AssertImageAsync("flowers_hsl.png", image);
        }

        [TestMethod]
        public async Task GradientMapAdjustment()
        {
            AddAdjustment(new GradientMapAdjustment()
            {
                Shadows = Color.FromArgb(255, 12, 16, 68),
                Highlights = Color.FromArgb(255, 233, 88, 228)
            });

            await AssertImageAsync("flowers_gm.png", image);
        }

        [TestMethod]
        public async Task BrightnessContrastAdjustment()
        {
            AddAdjustment(new BrightnessContrastAdjustment()
            {
                Brightness = 0.5f,
                Contrast = 0.5f
            });

            await AssertImageAsync("flowers_bc.png", image);
        }

        private void AddAdjustment(Adjustment adjustment)
        {
            image.Layers.Add(new AdjustmentLayer(adjustment));
        }
    }
}
