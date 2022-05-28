using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Foundation;

namespace Fotografix
{
    [TestClass]
    public class PhotoAdjustmentTest
    {
        private PhotoAdjustment adjustment;

        [TestInitialize]
        public void Initialize()
        {
            this.adjustment = new PhotoAdjustment();
        }

        [TestCleanup]
        public void Cleanup()
        {
            adjustment.Dispose();
        }

        /// <summary>
        /// The translation matrix for rotation depends on the size of the source photo.
        /// During deserialization, the rotation value is populated before the source is
        /// set. Ensure that we handle this scenario properly.
        /// </summary>
        [TestMethod]
        public void UpdatesRotationAfterSourceIsSet()
        {
            using (var bitmap = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), 200, 100, 96))
            {
                adjustment.Rotation = 90;
                adjustment.Source = bitmap;

                Assert.AreEqual(new Size(100, 200), adjustment.GetOutputSize());
            }
        }

        /// <summary>
        /// Rotation is only supported in 90-degree increments. Other values are invalid
        /// and we just fall back to zero rotation (we don't want to blow up because of
        /// one bad setting in an adjustment file - that would be a poor user experience).
        /// </summary>
        [TestMethod]
        public void IgnoresInvalidRotationValues()
        {
            adjustment.Rotation = 15;

            Assert.AreEqual(0, adjustment.Rotation);
        }

        [TestMethod]
        public void ResetsCropWhenRotationIsChanged()
        {
            adjustment.Crop = new CropRect(10, 20, 30, 40);
            adjustment.Rotation = 90;

            Assert.IsNull(adjustment.Crop);
        }

        [TestMethod]
        public void ComputesAdjustedSizes()
        {
            using (var bitmap = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), 200, 100, 96))
            {
                adjustment.Source = bitmap;
                adjustment.Rotation = 90;
                adjustment.Crop = new CropRect(10, 20, 30, 40);

                Assert.AreEqual(new Size(100, 200), adjustment.GetOrientedSize());
                Assert.AreEqual(new Size(30, 40), adjustment.GetOutputSize());
            }
        }
    }
}
