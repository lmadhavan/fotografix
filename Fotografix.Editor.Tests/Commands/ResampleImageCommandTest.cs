﻿using Fotografix.Adjustments;
using Fotografix.Editor.Commands;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tests.Commands
{
    [TestFixture]
    public class ResampleImageCommandTest
    {
        [Test]
        public void ChangesImageSize()
        {
            Size originalImageSize = new Size(50, 25);
            Image image = new Image(originalImageSize);

            Size newImageSize = new Size(100, 50);
            IChange change = new ResampleImageCommand(image, newImageSize, new FakeResamplingStrategy()).PrepareChange();

            change.Apply();
            Assert.That(image.Size, Is.EqualTo(newImageSize));

            change.Undo();
            Assert.That(image.Size, Is.EqualTo(originalImageSize));
        }

        [Test]
        public void ResamplesBitmapsProportionalToImageSize()
        {
            Size originalImageSize = new Size(50, 25);
            Image image = new Image(originalImageSize);

            Size originalBitmapSize = new Size(10, 20);
            BitmapLayer layer = new BitmapLayer(new Bitmap(originalBitmapSize));
            image.Layers.Add(layer);

            Size newImageSize = new Size(100, 50); // 200% of original image size
            IChange change = new ResampleImageCommand(image, newImageSize, new FakeResamplingStrategy()).PrepareChange();

            Size expectedNewBitmapSize = new Size(20, 40); // 200% of original bitmap size

            change.Apply();
            Assert.That(layer.Bitmap.Size, Is.EqualTo(expectedNewBitmapSize));

            change.Undo();
            Assert.That(layer.Bitmap.Size, Is.EqualTo(originalBitmapSize));
        }

        [Test]
        public void DoesNotAffectAdjustmentLayers()
        {
            Size originalImageSize = new Size(50, 25);
            Image image = new Image(originalImageSize);

            AdjustmentLayer layer = new AdjustmentLayer(new BlackAndWhiteAdjustment());
            image.Layers.Add(layer);

            Size newImageSize = new Size(100, 50);
            IChange change = new ResampleImageCommand(image, newImageSize, new FakeResamplingStrategy()).PrepareChange();

            change.Apply();
            Assert.That(image.Layers[0], Is.EqualTo(layer));
        }

        [Test]
        public void ProducesNoChangeIfImageSizeIsUnchanged()
        {
            Image image = new Image(new Size(50, 25));

            IChange change = new ResampleImageCommand(image, image.Size, new FakeResamplingStrategy()).PrepareChange();

            Assert.IsNull(change);
        }

        private sealed class FakeResamplingStrategy : IBitmapResamplingStrategy
        {
            public Bitmap Resample(Bitmap bitmap, Size newSize)
            {
                return new Bitmap(newSize);
            }
        }
    }
}
