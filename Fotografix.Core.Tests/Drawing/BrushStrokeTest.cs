using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Drawing
{
    [TestFixture]
    public class BrushStrokeTest
    {
        [Test]
        public void ComputesBounds()
        {
            BrushStroke brushStroke = new BrushStroke(new Point(10, 10), 3, Color.Transparent);
            brushStroke.AddPoint(new Point(20, 20));
            brushStroke.AddPoint(new Point(15, 25));

            Assert.That(brushStroke.Bounds, Is.EqualTo(Rectangle.FromLTRB(8, 8, 22, 27)));
        }
    }
}
