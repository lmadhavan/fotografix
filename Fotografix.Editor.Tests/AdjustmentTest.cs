using Fotografix.Editor.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Editor.Tests
{
    [TestClass]
    public class AdjustmentTest : EditorTestBase
    {
        private Image image;
        private Adjustment adjustment;

        [TestInitialize]
        public async Task Initialize()
        {
            this.image = await LoadImageAsync("flowers.jpg");
            this.adjustment = new BlackAndWhiteAdjustment();
            image.AddAdjustment(adjustment);
        }

        [TestCleanup]
        public void Cleanup()
        {
            image.Dispose();
            adjustment.Dispose();
        }

        [TestMethod]
        public async Task BlendMode_Normal_Opacity_100()
        {
            adjustment.BlendMode = BlendMode.Normal;
            adjustment.Opacity = 1;

            await AssertImageAsync("flowers_bw.png", image);
        }

        [TestMethod]
        public async Task BlendMode_DoesNotMatter_Opacity_0()
        {
            adjustment.Opacity = 0;

            await AssertImageAsync("flowers.jpg", image);
        }

        [TestMethod]
        public async Task BlendMode_Normal_Opacity_50()
        {
            adjustment.BlendMode = BlendMode.Normal;
            adjustment.Opacity = 0.5f;

            await AssertImageAsync("flowers_bw_opacity50.png", image);
        }

        [TestMethod]
        public async Task BlendMode_Multiply_Opacity_100()
        {
            adjustment.BlendMode = BlendMode.Multiply;
            adjustment.Opacity = 1;

            await AssertImageAsync("flowers_bw_multiply.png", image);
        }

        [TestMethod]
        public async Task BlendMode_Multiply_Opacity_50()
        {
            adjustment.BlendMode = BlendMode.Multiply;
            adjustment.Opacity = 0.5f;

            await AssertImageAsync("flowers_bw_multiply_opacity50.png", image);
        }
    }
}
