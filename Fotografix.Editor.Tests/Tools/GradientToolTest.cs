using Fotografix.Drawing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class GradientToolTest : DrawingToolTest
    {
        private GradientTool tool;

        protected override ITool Tool => tool;

        [SetUp]
        public void SetUp()
        {
            this.tool = new GradientTool()
            {
                StartColor = Color.Red,
                EndColor = Color.Green
            };
        }

        protected override void AssertDrawable(IDrawable drawable, PointerState start, PointerState end)
        {
            Assert.That(drawable, Is.InstanceOf<LinearGradient>());

            LinearGradient gradient = (LinearGradient)drawable;
            Assert.That(gradient.Bounds, Is.EqualTo(new Rectangle(Point.Empty, Image.Size)));
            Assert.That(gradient.StartColor, Is.EqualTo(tool.StartColor));
            Assert.That(gradient.EndColor, Is.EqualTo(tool.EndColor));
            Assert.That(gradient.StartPoint, Is.EqualTo(start.Location));
            Assert.That(gradient.EndPoint, Is.EqualTo(end.Location));
        }
    }
}
