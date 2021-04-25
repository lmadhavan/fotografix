using Fotografix.Adjustments;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public abstract class BitmapToolTest
    {
        protected Image Image { get; private set; }
        protected Bitmap Bitmap { get; private set; }
        protected Layer BitmapLayer { get; private set; }
        protected Layer NonBitmapLayer { get; private set; }

        protected abstract ITool Tool { get; }

        [SetUp]
        public void SetUp_BitmapToolTest()
        {
            this.Image = new Image(new Size(10, 10));
            this.Bitmap = new Bitmap(new Size(10, 10));
            this.BitmapLayer = new Layer(Bitmap);
            this.NonBitmapLayer = new Layer(new BlackAndWhiteAdjustment());
        }

        [Test]
        public void DisabledWhenNotActivated()
        {
            Assert.That(Tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void DisabledWhenActiveLayerIsNotBitmapLayer()
        {
            Activate(NonBitmapLayer);

            Assert.That(Tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void EnabledWhenActiveLayerIsBitmapLayer()
        {
            Activate(BitmapLayer);

            Assert.That(Tool.Cursor, Is.Not.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void DisabledWhenActiveLayerChangesToNonBitmapLayerAfterActivation()
        {
            Activate(BitmapLayer);
            Image.SetActiveLayer(NonBitmapLayer);

            Assert.That(Tool.Cursor, Is.EqualTo(ToolCursor.Disabled));
        }

        [Test]
        public void IgnoresPointerEventsWhenDisabled()
        {
            PointerState Start = new(10, 10);
            PointerState End = new(20, 20);

            Tool.PointerPressed(Start);
            Tool.PointerMoved(End);
            Tool.PointerReleased(End);

            Assert.Pass();
        }

        protected void Activate(Layer layer)
        {
            Image.SetActiveLayer(layer);
            Tool.Activated(Image);
        }
    }
}
