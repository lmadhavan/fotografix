using Fotografix.Adjustments;
using NUnit.Framework;
using System;
using System.Drawing;

namespace Fotografix.Editor
{
    [TestFixture]
    public class ImageChangeTrackerTest
    {
        private Image image;
        private Bitmap bitmap;
        private BitmapLayer bitmapLayer;
        private HueSaturationAdjustment adjustment;
        private AdjustmentLayer adjustmentLayer;

        private ImageChangeTracker tracker;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(new Size(10, 10));

            this.bitmap = new Bitmap(new Size(5, 5));
            this.bitmapLayer = new BitmapLayer(bitmap);
            image.Layers.Add(bitmapLayer);
            
            this.adjustment = new HueSaturationAdjustment();
            this.adjustmentLayer = new AdjustmentLayer(adjustment);
            image.Layers.Add(adjustmentLayer);

            this.tracker = new ImageChangeTracker(image);
        }

        [TearDown]
        public void TearDown()
        {
            tracker.Dispose();
        }

        [Test]
        public void TracksImageSizeChange()
        {
            AssertContentChanged(true, () => image.Size = new Size(20, 20));
        }

        [Test]
        public void TracksLayerRemoval()
        {
            AssertContentChanged(true, () => image.Layers.Remove(bitmapLayer));
        }

        [Test]
        public void TracksLayerPropertyChange()
        {
            AssertContentChanged(true, () => bitmapLayer.Opacity = 0.5f);
        }

        [Test]
        public void StopsTrackingLayerAfterRemoval()
        {
            image.Layers.Remove(bitmapLayer);
            AssertContentChanged(false, () => bitmapLayer.Opacity = 0.5f);
        }

        [Test]
        public void StopsTrackingLayerAfterClear()
        {
            image.Layers.Clear();
            AssertContentChanged(false, () => bitmapLayer.Opacity = 0.5f);
        }

        [Test]
        public void TracksBitmapContentChange()
        {
            AssertContentChanged(true, () => bitmapLayer.Bitmap.Invalidate());
        }

        [Test]
        public void TracksAdjustmentPropertyChange()
        {
            AssertContentChanged(true, () => adjustment.Hue = 0.5f);
        }

        private void AssertContentChanged(bool expected, Action action)
        {
            bool contentChanged = false;
            tracker.ContentChanged += (s, e) => contentChanged = true;

            action();
            Assert.AreEqual(expected, contentChanged);
        }
    }
}
