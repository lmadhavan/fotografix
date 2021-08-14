using Fotografix.Adjustments;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Acceptance
{
    [TestClass]
    public class AdjustmentsAcceptanceTest : AcceptanceTestBase
    {
        [TestMethod]
        public async Task BlackAndWhiteAdjustment()
        {
            await OpenImageAsync("flowers.jpg");

            await AddAdjustmentLayerAsync<BlackAndWhiteAdjustment>();

            await AssertImageAsync("flowers_bw.png");
        }

        [TestMethod]
        public async Task BrightnessContrastAdjustment()
        {
            await OpenImageAsync("flowers.jpg");

            await AddAdjustmentLayerAsync<BrightnessContrastAdjustment>();
            SetAdjustmentProperties<BrightnessContrastAdjustment>(p =>
            {
                p.Brightness = 0.5f;
                p.Contrast = 0.5f;
            });

            await AssertImageAsync("flowers_bc.png");
        }

        [TestMethod]
        public async Task GradientMapAdjustment()
        {
            await OpenImageAsync("flowers.jpg");

            await AddAdjustmentLayerAsync<GradientMapAdjustment>();
            SetAdjustmentProperties<GradientMapAdjustment>(p =>
            {
                p.Shadows = Color.FromArgb(255, 12, 16, 68);
                p.Highlights = Color.FromArgb(255, 233, 88, 228);
            });

            await AssertImageAsync("flowers_gm.png");
        }

        [TestMethod]
        public async Task HueSaturationAdjustment()
        {
            await OpenImageAsync("flowers.jpg");

            await AddAdjustmentLayerAsync<HueSaturationAdjustment>();
            SetAdjustmentProperties<HueSaturationAdjustment>(p =>
            {
                p.Hue = 0.5f;
                p.Saturation = 0.25f;
                p.Lightness = 0.25f;
            });

            await AssertImageAsync("flowers_hsl.png");
        }

        [TestMethod]
        public async Task LevelsAdjustment()
        {
            await OpenImageAsync("flowers.jpg");

            await AddAdjustmentLayerAsync<LevelsAdjustment>();
            SetAdjustmentProperties<LevelsAdjustment>(p =>
            {
                p.InputBlackPoint = 0.2f;
                p.InputWhitePoint = 0.8f;
                p.InputGamma = 2f;
                p.OutputBlackPoint = 0.1f;
                p.OutputWhitePoint = 0.9f;
            });

            await AssertImageAsync("flowers_levels.png");
        }

        private Task AddAdjustmentLayerAsync<T>() where T : Adjustment, new()
        {
            return Workspace.NewAdjustmentLayerCommand.ExecuteAsync(typeof(T));
        }

        private void SetAdjustmentProperties<T>(Action<T> action) where T : Adjustment
        {
            T adjustment = (T)Editor.ActiveLayer.Content;
            Assert.IsNotNull(adjustment, typeof(T).Name);
            action(adjustment);
        }
    }
}
