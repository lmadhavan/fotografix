using Fotografix.Adjustments;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    public abstract class ChannelToolTest
    {
        private Document document;

        protected Image Image { get; private set; }
        protected Layer BitmapLayer { get; private set; }
        protected Layer NonBitmapLayer { get; private set; }
        protected Channel ActiveChannel { get; private set; }

        protected abstract ITool Tool { get; }

        [SetUp]
        public void SetUp_ChannelToolTest()
        {
            this.Image = new Image(new Size(10, 10));
            this.BitmapLayer = new Layer(new Bitmap(Size.Empty));
            this.NonBitmapLayer = new Layer(new BlackAndWhiteAdjustment());

            this.document = new Document(Image);
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
            document.ActiveLayer = NonBitmapLayer;

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
            document.ActiveLayer = layer;
            Tool.Activated(document);
            this.ActiveChannel = layer.ContentChannel;
        }
    }
}
