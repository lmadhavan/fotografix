using Fotografix.Testing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Core.Tests
{
    [TestFixture]
    public class BitmapLayerTest
    {
        private static readonly byte[] OriginalBytes = new byte[] { 1, 2, 3, 4 };
        private static readonly byte[] PaintedBytes = new byte[] { 5, 6, 7, 8 };

        private PaintableFakeBitmap bitmap;
        private BitmapLayer layer;
        private BrushStroke brushStroke;

        [SetUp]
        public void SetUp()
        {
            this.bitmap = new PaintableFakeBitmap();
            bitmap.SetPixelBytes(OriginalBytes);
            bitmap.SetPixelBytesOnPaint(PaintedBytes);

            this.layer = new BitmapLayer(bitmap);
            this.brushStroke = new BrushStroke(new PointF(10, 10), 5, Color.White);
        }

        [Test]
        public void PaintsBrushStrokeOnBitmap()
        {
            layer.Paint(brushStroke);

            Assert.That(bitmap.LastBrushStroke, Is.SameAs(brushStroke));
            Assert.That(bitmap.GetPixelBytes(), Is.EqualTo(PaintedBytes));
        }

        [Test]
        public void PaintingBrushStrokeIsUndoable()
        {
            IUndoable undoable = layer.Paint(brushStroke);
            undoable.Undo();

            Assert.That(bitmap.GetPixelBytes(), Is.EqualTo(OriginalBytes));
        }

        [Test]
        public void PaintingBrushStrokeRaisesContentChanged()
        {
            Assert.That(layer, Raises.ContentChanged.When(() => layer.Paint(brushStroke)));
        }

        [Test]
        public void UndoingBrushStrokeRaisesContentChanged()
        {
            IUndoable undoable = layer.Paint(brushStroke);
            Assert.That(layer, Raises.ContentChanged.When(() => undoable.Undo()));
        }

        private class PaintableFakeBitmap : FakeBitmap
        {
            private byte[] paintedBytes;

            public BrushStroke LastBrushStroke { get; private set; }

            public override void Paint(BrushStroke brushStroke)
            {
                this.LastBrushStroke = brushStroke;
                SetPixelBytes(paintedBytes);
            }

            internal void SetPixelBytesOnPaint(byte[] paintedBytes)
            {
                this.paintedBytes = paintedBytes;
            }
        }
    }
}
