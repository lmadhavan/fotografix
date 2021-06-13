using Fotografix.Drawing;
using Fotografix.Win2D;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Tests.Composition
{
    [TestClass]
    public class DrawingContextTest
    {
        private IGraphicsDevice graphicsDevice;

        [TestInitialize]
        public void Initialize()
        {
            this.graphicsDevice = new Win2DGraphicsDevice();
        }

        [TestMethod]
        public async Task DrawsInImageCoordinates()
        {
            Bitmap bitmap = await TestImages.LoadBitmapAsync("stars_small.jpg");
            bitmap.Position = new Point(25, 25);

            BrushStroke brushStroke = new BrushStroke(
                points: new Point[] { new Point(25, 25), new Point(125, 125) },
                size: 5,
                color: Color.White
            );

            using (IDrawingContext dc = graphicsDevice.CreateDrawingContext(bitmap))
            {
                dc.Draw(brushStroke);
            }

            await BitmapAssert.AreEquivalentAsync("stars_small_offset_brush.png", bitmap);
        }
    }
}
