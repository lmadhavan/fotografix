using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Fotografix
{
    [TestClass]
    public class PhotoAdjustmentTest
    {
        private CanvasBitmap bitmap;
        private PhotoAdjustment adjustment;

        [TestInitialize]
        public async Task Initialize()
        {
            var file = await TestData.GetFileAsync("Photos\\Barn.jpg");

            using (var stream = await file.OpenReadAsync())
            {
                this.bitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), stream);
            }

            this.adjustment = new PhotoAdjustment { Source = bitmap };
        }

        [TestCleanup]
        public void Cleanup()
        {
            adjustment.Dispose();
            bitmap.Dispose();
        }

        [TestMethod]
        public async Task Exposure()
        {
            adjustment.Exposure = 0.5f;
            await VerifyOutputAsync("Barn_exposure.jpg");
        }

        [TestMethod]
        public async Task Contrast()
        {
            adjustment.Contrast = 0.5f;
            await VerifyOutputAsync("Barn_contrast.jpg");
        }

        [TestMethod]
        public async Task Highlights()
        {
            adjustment.Highlights = 0.5f;
            await VerifyOutputAsync("Barn_highlights.jpg");
        }

        [TestMethod]
        public async Task Shadows()
        {
            adjustment.Shadows = 0.5f;
            await VerifyOutputAsync("Barn_shadows.jpg");
        }

        [TestMethod]
        public async Task Whites()
        {
            adjustment.Whites = 0.5f;
            await VerifyOutputAsync("Barn_whites.jpg");
        }

        [TestMethod]
        public async Task Blacks()
        {
            adjustment.Blacks = -0.5f;
            await VerifyOutputAsync("Barn_blacks.jpg");
        }

        [TestMethod]
        public async Task Temperature()
        {
            adjustment.Temperature = 0.5f;
            await VerifyOutputAsync("Barn_temperature.jpg");
        }

        [TestMethod]
        public async Task Tint()
        {
            adjustment.Tint = 0.5f;
            await VerifyOutputAsync("Barn_tint.jpg");
        }

        [TestMethod]
        public async Task Clarity()
        {
            adjustment.Clarity = 0.5f;
            await VerifyOutputAsync("Barn_clarity.jpg");
        }

        [TestMethod]
        public async Task Clarity_Scaled()
        {
            adjustment.Clarity = 0.5f;
            adjustment.RenderScale = 0.5f;
            await VerifyOutputAsync("Barn_clarity_scaled.jpg");
        }

        [TestMethod]
        public async Task Vibrance()
        {
            adjustment.Vibrance = 1f;
            await VerifyOutputAsync("Barn_vibrance.jpg");
        }

        [TestMethod]
        public async Task Saturation()
        {
            adjustment.Saturation = 1f;
            await VerifyOutputAsync("Barn_saturation.jpg");
        }

        [TestMethod]
        public async Task ColorRange_Hue()
        {
            adjustment.ColorRanges.HueView.Yellow = -0.5f;
            await VerifyOutputAsync("Barn_hue_yellow.jpg");
        }

        [TestMethod]
        public async Task ColorRange_Saturation()
        {
            adjustment.ColorRanges.SaturationView.Yellow = 1f;
            await VerifyOutputAsync("Barn_saturation_yellow.jpg");
        }

        [TestMethod]
        public async Task ColorRange_Luminance()
        {
            adjustment.ColorRanges.LuminanceView.Yellow = 1f;
            await VerifyOutputAsync("Barn_luminance_yellow.jpg");
        }

        [TestMethod]
        public async Task Sharpness()
        {
            adjustment.Sharpness.Amount = 0.5f;
            adjustment.Sharpness.Radius = 2f;
            adjustment.Sharpness.Threshold = 0.05f;
            await VerifyOutputAsync("Barn_sharpness.jpg");
        }

        [TestMethod]
        public async Task Sharpness_Scaled()
        {
            adjustment.Sharpness.Amount = 0.5f;
            adjustment.Sharpness.Radius = 2f;
            adjustment.Sharpness.Threshold = 0.05f;
            adjustment.RenderScale = 0.5f;
            await VerifyOutputAsync("Barn_sharpness_scaled.jpg");
        }

        [TestMethod]
        public async Task BlackAndWhite()
        {
            // set up some initial color adjustments -- these should be reset when entering B&W mode
            adjustment.Vibrance = 0.5f;
            adjustment.Saturation = 0.5f;
            adjustment.ColorRanges.HueView.Yellow = -0.5f;
            adjustment.ColorRanges.SaturationView.Yellow = 1f;

            adjustment.BlackAndWhite = true;
            adjustment.ColorRanges.LuminanceView.Yellow = 1f;
            await VerifyOutputAsync("Barn_bw.jpg");

            // luminance values should be reset when exiting B&W mode
            adjustment.BlackAndWhite = false;
            Assert.AreEqual(0, adjustment.ColorRanges.LuminanceView.Yellow);
        }

        [TestMethod]
        public async Task Crop()
        {
            adjustment.Crop = new Rect(450, 0, 450, 600);
            await VerifyOutputAsync("Barn_crop.jpg");
        }

        [TestMethod]
        public async Task Crop_Scaled()
        {
            adjustment.Crop = new Rect(450, 0, 450, 600);
            adjustment.RenderScale = 0.5f;
            await VerifyOutputAsync("Barn_crop_scaled.jpg");
        }

        private async Task VerifyOutputAsync(string filename, float tolerance = BitmapAssert.DefaultTolerance)
        {
            using (var output = new CanvasRenderTarget(bitmap, adjustment.GetOutputSize(bitmap)))
            {
                using (var ds = output.CreateDrawingSession())
                {
                    ds.DrawImage(adjustment.Output);
                }

                await BitmapAssert.VerifyAsync(output, filename, tolerance);
            }
        }
    }
}
