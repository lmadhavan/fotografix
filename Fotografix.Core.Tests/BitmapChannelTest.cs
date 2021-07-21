using Fotografix.Drawing;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix
{
    [TestFixture]
    public class BitmapChannelTest
    {
        private TestGraphicsDevice device;

        [SetUp]
        public void SetUp()
        {
            this.device = new();
        }

        [Test]
        public void DrawsDrawableOnBitmap()
        {
            Bitmap bitmap = new Bitmap(new Size(10, 10));
            TestDrawable drawable = new(bitmap.Bounds);

            BitmapChannel channel = new BitmapChannel(bitmap);
            channel.Draw(drawable, device);

            device.VerifySequence(
                ("CreateDrawingContext", bitmap),
                ("Draw", drawable),
                ("DisposeDrawingContext")
            );
        }

        [Test]
        public void ExpandsBitmapToAccommodateDrawable()
        {
            Bitmap bitmap = new Bitmap(new Rectangle(10, 10, 20, 20));
            TestDrawable drawable = new(new Rectangle(5, 5, 10, 10));

            BitmapChannel channel = new BitmapChannel(bitmap);
            channel.Draw(drawable, device);

            Bitmap newBitmap = channel.Bitmap;
            Assert.That(newBitmap.Bounds, Is.EqualTo(Rectangle.FromLTRB(5, 5, 30, 30)));

            device.VerifySequence(
                ("CreateDrawingContext", newBitmap),
                ("Draw", bitmap, bitmap.Bounds, bitmap.Bounds),
                ("Draw", drawable),
                ("DisposeDrawingContext")
            );
        }

        [Test]
        public void IgnoresExistingBitmapIfEmpty()
        {
            Bitmap bitmap = new Bitmap(Size.Empty);

            Rectangle drawableBounds = new Rectangle(5, 5, 10, 10);
            TestDrawable drawable = new(drawableBounds);

            BitmapChannel channel = new BitmapChannel(bitmap);
            channel.Draw(drawable, device);

            Bitmap newBitmap = channel.Bitmap;
            Assert.That(newBitmap.Bounds, Is.EqualTo(drawableBounds));

            device.VerifySequence(
                ("CreateDrawingContext", newBitmap),
                ("Draw", drawable),
                ("DisposeDrawingContext")
            );
        }

        [Test]
        public void ScalesBitmapBySpecifiedFactor()
        {
            Bitmap bitmap = new Bitmap(new Rectangle(10, 10, 20, 20));
            BitmapChannel channel = new BitmapChannel(bitmap);

            channel.Scale(new(1.5f, 1.5f), device);

            Bitmap newBitmap = channel.Bitmap;
            Assert.That(newBitmap.Bounds, Is.EqualTo(new Rectangle(15, 15, 30, 30)));

            device.VerifySequence(
                ("CreateDrawingContext", newBitmap),
                ("Draw", bitmap, newBitmap.Bounds, bitmap.Bounds),
                ("DisposeDrawingContext")
            );
        }

        [Test]
        public void RepositionsBitmapToCropRectangle()
        {
            Bitmap bitmap = new Bitmap(new Rectangle(10, 10, 20, 20));
            BitmapChannel channel = new BitmapChannel(bitmap);

            channel.Crop(new Rectangle(25, 25, 50, 50));

            Assert.That(bitmap.Position, Is.EqualTo(new Point(-15, -15)));
        }
    }
}
