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
            AddAdjustment(AdjustmentFactory.CreateBlackAndWhiteAdjustment());

            await AssertImageAsync("flowers_bw.png", image);
        }

        [TestMethod]
        public async Task HueSaturationAdjustment()
        {
            var adjustment = AdjustmentFactory.CreateHueSaturationAdjustment();
            AddAdjustment(adjustment);

            adjustment.Hue = 0.5f;
            adjustment.Saturation = 0.25f;
            adjustment.Lightness = 0.25f;

            await AssertImageAsync("flowers_hsl.png", image);
        }

        [TestMethod]
        public async Task GradientMapAdjustment()
        {
            var adjustment = AdjustmentFactory.CreateGradientMapAdjustment();
            AddAdjustment(adjustment);

            adjustment.Shadows = Color.FromArgb(255, 12, 16, 68);
            adjustment.Highlights = Color.FromArgb(255, 233, 88, 228);

            await AssertImageAsync("flowers_gm.png", image);
        }

        [TestMethod]
        public async Task BrightnessContrastAdjustment()
        {
            var adjustment = AdjustmentFactory.CreateBrightnessContrastAdjustment();
            AddAdjustment(adjustment);

            adjustment.Brightness = 0.5f;
            adjustment.Contrast = 0.5f;

            await AssertImageAsync("flowers_bc.png", image);
        }

        private void AddAdjustment(IAdjustment adjustment)
        {
            image.Layers.Add(new AdjustmentLayer(adjustment));
        }
    }
}
