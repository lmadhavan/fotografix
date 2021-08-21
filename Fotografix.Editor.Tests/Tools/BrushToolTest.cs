using Fotografix.Drawing;
using Fotografix.Editor.Colors;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class BrushToolTest : DrawingToolTest
    {
        private static readonly Color BrushColor = Color.Red;

        private BrushTool tool;

        protected override ITool Tool => tool;

        [SetUp]
        public void SetUp()
        {
            var colors = new ColorControls { ForegroundColor = BrushColor };

            this.tool = new BrushTool(colors, DrawCommand)
            {
                Size = 5
            };
        }

        protected override void AssertDrawable(IDrawable drawable, PointerState start, PointerState end)
        {
            Assert.That(drawable, Is.InstanceOf<BrushStroke>());

            BrushStroke brushStroke = (BrushStroke)drawable;
            Assert.That(brushStroke.Size, Is.EqualTo(tool.Size));
            Assert.That(brushStroke.Color, Is.EqualTo(BrushColor));
            Assert.That(brushStroke.Points, Is.EqualTo(new Point[] { start.Location, end.Location }).AsCollection);
        }
    }
}
