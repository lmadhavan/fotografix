using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Fotografix
{
    [TestClass]
    public class PhotoAdjustmentSerializationTest
    {
        [TestMethod]
        public async Task RoundTrip()
        {
            var file = await TestData.GetFileAsync("adjustment.json");
            var str = File.ReadAllText(file.Path);

            using (PhotoAdjustment adjustment = PhotoAdjustment.Deserialize(str))
            {
                Assert.AreEqual(str, adjustment.Serialize());
            }
        }

        [Ignore]
        [TestMethod]
        public void Serialize()
        {
            using (PhotoAdjustment adjustment = new PhotoAdjustment())
            {
                adjustment.Exposure = 0.1f;

                adjustment.ColorRanges[ColorRange.Red].Hue = 0.2f;
                adjustment.ColorRanges[ColorRange.Green].Saturation = 0.3f;
                adjustment.ColorRanges[ColorRange.Blue].Luminance = 0.4f;

                adjustment.Sharpness.Amount = 0.5f;

                Debug.WriteLine(adjustment.Serialize());
            }
        }
    }
}
