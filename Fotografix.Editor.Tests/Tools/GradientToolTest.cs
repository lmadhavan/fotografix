using Fotografix.Drawing;
using Fotografix.Editor.Colors;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class GradientToolTest : DrawingToolTest
    {
        private static readonly Color StartColor = Color.Red;
        private static readonly Color EndColor = Color.Green;

        private GradientTool tool;

        protected override ITool Tool => tool;

        [SetUp]
        public void SetUp()
        {
            var colors = new ColorControls
            {
                ForegroundColor = StartColor,
                BackgroundColor = EndColor
            };

            this.tool = new GradientTool(colors, DrawCommand);
        }

        protected override void AssertDrawable(IDrawable drawable, PointerState start, PointerState end)
        {
            Assert.That(drawable, Is.InstanceOf<LinearGradient>());

            LinearGradient gradient = (LinearGradient)drawable;
            Assert.That(gradient.Bounds, Is.EqualTo(new Rectangle(Point.Empty, Image.Size)));
            Assert.That(gradient.StartColor, Is.EqualTo(StartColor));
            Assert.That(gradient.EndColor, Is.EqualTo(EndColor));
            Assert.That(gradient.StartPoint, Is.EqualTo(start.Location));
            Assert.That(gradient.EndPoint, Is.EqualTo(end.Location));
        }
    }
}
