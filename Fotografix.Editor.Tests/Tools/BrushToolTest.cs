using Fotografix.Drawing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class BrushToolTest : DrawingToolTest
    {
        private BrushTool tool;

        protected override ITool Tool => tool;

        [SetUp]
        public void SetUp()
        {
            this.tool = new BrushTool()
            {
                Size = 5,
                Color = Color.Red
            };
        }

        protected override void AssertDrawable(IDrawable drawable, PointerState start, PointerState end)
        {
            Assert.That(drawable, Is.InstanceOf<BrushStroke>());

            BrushStroke brushStroke = (BrushStroke)drawable;
            Assert.That(brushStroke.Size, Is.EqualTo(tool.Size));
            Assert.That(brushStroke.Color, Is.EqualTo(tool.Color));
            Assert.That(brushStroke.Points, Is.EqualTo(new Point[] { start.Location, end.Location }).AsCollection);
        }
    }
}
