using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class BitmapBlendingTest : CompositionTestBase
    {
        private Image image;
        private Layer background;
        private Layer foreground;

        [TestInitialize]
        public async Task Initialize()
        {
            this.image = await LoadImageAsync("flowers.jpg");
            this.background = image.Layers[0];
            this.foreground = await LoadLayerAsync("stars.jpg");
            image.Layers.Add(foreground);
        }

        [TestMethod]
        public async Task DefaultProperties()
        {
            await AssertImageAsync("stars.jpg", image);
        }

        [TestMethod]
        public async Task ForegroundSmallerThanBackground()
        {
            image.Layers[1] = await LoadLayerAsync("stars_small.jpg");

            await AssertImageAsync("flowers_stars_small.png", image);
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

            await AssertImageAsync("flowers_stars_opacity50.png", image);
        }

        [TestMethod]
        public async Task Foreground_BlendMode_Screen()
        {
            foreground.BlendMode = BlendMode.Screen;

            await AssertImageAsync("flowers_stars_screen.png", image);
        }

        [TestMethod]
        public async Task Foreground_BlendMode_Screen_Opacity_50()
        {
            foreground.BlendMode = BlendMode.Screen;
            foreground.Opacity = 0.5f;

            await AssertImageAsync("flowers_stars_screen_opacity50.png", image);
        }

        [TestMethod]
        public async Task Background_Visible_False()
        {
            background.Visible = false;

            await AssertImageAsync("stars.jpg", image);
        }

        [TestMethod]
        public async Task Background_Opacity_0()
        {
            background.Opacity = 0;

            await AssertImageAsync("stars.jpg", image);
        }

        [TestMethod]
        public async Task Background_Opacity_50()
        {
            background.Opacity = 0.5f;

            await AssertImageAsync("stars.jpg", image);
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
