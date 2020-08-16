using Fotografix.Testing;
using NUnit.Framework;

namespace Fotografix.Core.Tests
{
    [TestFixture]
    public class BitmapLayerTest
    {
        private static readonly byte[] OriginalBytes = new byte[] { 1, 2, 3, 4 };
        private static readonly byte[] PaintedBytes = new byte[] { 5, 6, 7, 8 };

        private PaintableFakeBitmap bitmap;
        private BitmapLayer layer;
        private IDrawable drawable;

        [SetUp]
        public void SetUp()
        {
            this.bitmap = new PaintableFakeBitmap();
            bitmap.SetPixelBytes(OriginalBytes);
            bitmap.SetPixelBytesOnPaint(PaintedBytes);

            this.layer = new BitmapLayer(bitmap);
            this.drawable = new FakeDrawable();
        }

        [Test]
        public void DrawsDrawableOnBitmap()
        {
            layer.Draw(drawable);

            Assert.That(bitmap.LastDrawable, Is.SameAs(drawable));
            Assert.That(bitmap.GetPixelBytes(), Is.EqualTo(PaintedBytes));
        }

        [Test]
        public void DrawingIsUndoable()
        {
            IUndoable undoable = layer.Draw(drawable);
            undoable.Undo();

            Assert.That(bitmap.GetPixelBytes(), Is.EqualTo(OriginalBytes));
        }

        [Test]
        public void DrawingRaisesContentChanged()
        {
            Assert.That(layer, Raises.ContentChanged.When(() => layer.Draw(drawable)));
        }

        [Test]
        public void UndoingDrawingRaisesContentChanged()
        {
            IUndoable undoable = layer.Draw(drawable);
            Assert.That(layer, Raises.ContentChanged.When(() => undoable.Undo()));
        }

        private class PaintableFakeBitmap : FakeBitmap
        {
            private byte[] paintedBytes;

            public IDrawable LastDrawable { get; private set; }

            public override void Draw(IDrawable drawable)
            {
                this.LastDrawable = drawable;
                SetPixelBytes(paintedBytes);
            }

            internal void SetPixelBytesOnPaint(byte[] paintedBytes)
            {
                this.paintedBytes = paintedBytes;
            }
        }

        private class FakeDrawable : NotifyContentChangedBase, IDrawable
        {
            public void Dispose()
            {
            }
        }
    }
}
