using Fotografix.Composition;
using Microsoft.Graphics.Canvas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class BitmapLayerTest : ImageTestBase
    {
        [TestMethod]
        public async Task LoadsBitmapFromFile()
        {
            const string filename = "flowers.jpg";

            var file = await GetFileAsync(filename);
            var layer = await BitmapLayer.LoadAsync(CanvasDevice.GetSharedDevice(), file);

            Assert.AreEqual(filename, layer.Name);
            Assert.AreEqual(320, layer.Width);
            Assert.AreEqual(480, layer.Height);
        }
    }
}
