using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class AdjustmentBlendingTest : CompositionTestBase
    {
        private Image image;
        private Layer background;
        private Layer foreground;

        [TestInitialize]
        public async Task Initialize()
        {
            this.image = await LoadImageAsync("flowers.jpg");
            this.background = image.Layers[0];
            this.foreground = new AdjustmentLayer(AdjustmentFactory.CreateBlackAndWhiteAdjustment());
            image.Layers.Add(foreground);
        }

        [TestMethod]
        public async Task DefaultProperties()
        {
            await AssertImageAsync("flowers_bw.png", image);
        }

        [TestMethod]
        public async Task Foreground_Visible_False()
        {
            foreground.Visible = false;

            await AssertImageAsync("flowers.jpg", image);
        }

        [TestMethod]
        public async Task Foreground_Opacity_0()
        {
            foreground.Opacity = 0;

            await AssertImageAsync("flowers.jpg", image);
        }

        [TestMethod]
        public async Task Foreground_Opacity_50()
        {
            foreground.Opacity = 0.5f;

            await AssertImageAsync("flowers_bw_opacity50.png", image);
        }

        [TestMethod]
        public async Task Foreground_BlendMode_Multiply()
        {
            foreground.BlendMode = BlendMode.Multiply;

            await AssertImageAsync("flowers_bw_multiply.png", image);
        }

        [TestMethod]
        public async Task Foreground_BlendMode_Multiply_Opacity_50()
        {
            foreground.BlendMode = BlendMode.Multiply;
            foreground.Opacity = 0.5f;

            await AssertImageAsync("flowers_bw_multiply_opacity50.png", image);
        }

        [TestMethod]
        public async Task Background_Visible_False()
        {
            background.Visible = false;

            await AssertImageAsync("empty.png", image);
        }

        [TestMethod]
        public async Task Background_Opacity_0()
        {
            background.Opacity = 0;

            await AssertImageAsync("empty.png", image);
        }

        [TestMethod]
        public async Task Background_Opacity_50()
        {
            background.Opacity = 0.5f;

            await AssertImageAsync("flowers_opacity50_bw.png", image);
        }

        [TestMethod]
        public async Task Both_Visible_False()
        {
            foreground.Visible = false;
            background.Visible = false;

            await AssertImageAsync("empty.png", image);
        }
    }
}
