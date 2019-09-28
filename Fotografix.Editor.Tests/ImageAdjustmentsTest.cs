using Fotografix.Editor.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Windows.UI;

namespace Fotografix.Editor.Tests
{
    [TestClass]
    public class ImageAdjustmentsTest : EditorTestBase
    {
        private Image image;

        [TestInitialize]
        public async Task Initialize()
        {
            this.image = await LoadImageAsync("flowers.jpg");
        }

        [TestCleanup]
        public void Cleanup()
        {
            image.Dispose();
        }

        [TestMethod]
        public async Task BlackAndWhiteAdjustment()
        {
            using (BlackAndWhiteAdjustment adjustment = new BlackAndWhiteAdjustment())
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_bw.png", image);
            }
        }

        [TestMethod]
        public async Task ShadowsHighlightsAdjustment()
        {
            using (ShadowsHighlightsAdjustment adjustment = new ShadowsHighlightsAdjustment()
            {
                Shadows = 0.25f,
                Highlights = -0.25f,
                Clarity = 0.5f
            })
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_sh.png", image);
            }
        }

        [TestMethod]
        public async Task HueSaturationAdjustment()
        {
            using (HueSaturationAdjustment adjustment = new HueSaturationAdjustment()
            {
                Hue = 0.5f,
                Saturation = 0.25f,
                Lightness = 0.25f
            })
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_hsl.png", image);
            }
        }

        [TestMethod]
        public async Task GradientMapAdjustment()
        {
            using (GradientMapAdjustment adjustment = new GradientMapAdjustment()
            {
                Shadows = Color.FromArgb(255, 12, 16, 68),
                Highlights = Color.FromArgb(255, 233, 88, 228)
            })
            {
                image.AddAdjustment(adjustment);
                await AssertImageAsync("flowers_gm.png", image);
            }
        }
    }
}
