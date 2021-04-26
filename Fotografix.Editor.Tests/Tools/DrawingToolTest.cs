using Fotografix.Drawing;
using Fotografix.Editor.Commands;
using Moq;
using NUnit.Framework;

namespace Fotografix.Editor.Tools
{
    public abstract class DrawingToolTest : BitmapToolTest
    {
        private static readonly PointerState Start = new(10, 10);
        private static readonly PointerState End = new(20, 20);

        private Mock<ICommandDispatcher> commandDispatcher;

        protected abstract void AssertDrawable(IDrawable drawable, PointerState start, PointerState end);

        [SetUp]
        public void SetUp_DrawingToolTest()
        {
            this.commandDispatcher = new Mock<ICommandDispatcher>();
            Image.SetCommandDispatcher(commandDispatcher.Object);
        }

        [Test]
        public void BeginsDrawingPreviewWhenPointerPressed()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            
            Assert.That(Bitmap.GetDrawingPreview(), Is.Not.Null);
        }

        [Test]
        public void UpdatesDrawingWhenPointerMoved()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            Tool.PointerMoved(End);

            AssertDrawable(Bitmap.GetDrawingPreview(), Start, End);
        }

        [Test]
        public void CommitsDrawingWhenPointerReleased()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            Tool.PointerMoved(End);

            IDrawable drawable = Bitmap.GetDrawingPreview();

            Tool.PointerReleased(End);

            commandDispatcher.Verify(d => d.Dispatch(new DrawCommand(BitmapLayer, drawable)));
            Assert.That(Bitmap.GetDrawingPreview(), Is.Null);
        }

        [Test]
        public void DoesNotUpdateDrawingAfterPointerReleased()
        {
            Activate(BitmapLayer);

            Tool.PointerPressed(Start);
            Tool.PointerMoved(End);

            IDrawable drawable = Bitmap.GetDrawingPreview();

            Tool.PointerReleased(End);
            Tool.PointerMoved(new(30, 30));

            AssertDrawable(drawable, Start, End);
        }
    }
}
