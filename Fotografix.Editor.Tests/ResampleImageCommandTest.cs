using Fotografix.Adjustments;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tests
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
            Command command = new ResampleImageCommand(image, newImageSize);

            command.Execute();
            Assert.That(image.Size, Is.EqualTo(newImageSize));

            command.Undo();
            Assert.That(image.Size, Is.EqualTo(originalImageSize));
        }

        [Test]
        public void ResamplesBitmapsProportionalToImageSize()
        {
            Size originalImageSize = new Size(50, 25);
            Image image = new Image(originalImageSize);

            Size originalBitmapSize = new Size(10, 20);
            BitmapLayer layer = new BitmapLayer(new FakeBitmap(originalBitmapSize));
            image.Layers.Add(layer);

            Size newImageSize = new Size(100, 50); // 200% of original image size
            Command command = new ResampleImageCommand(image, newImageSize);

            Size expectedNewBitmapSize = new Size(20, 40); // 200% of original bitmap size

            command.Execute();
            Assert.That(layer.Bitmap.Size, Is.EqualTo(expectedNewBitmapSize));

            command.Undo();
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
            Command command = new ResampleImageCommand(image, newImageSize);

            command.Execute();
            Assert.That(image.Layers[0], Is.EqualTo(layer));
        }
    }
}
