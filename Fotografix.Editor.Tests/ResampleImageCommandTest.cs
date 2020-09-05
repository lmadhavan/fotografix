using Fotografix.Testing;
using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor
{
    [TestFixture]
    public class ResampleImageCommandTest
    {
        private const int ScalingFactor = 2;
        private static readonly Size OriginalImageSize = new Size(50, 25);
        private static readonly Size NewImageSize = OriginalImageSize * ScalingFactor;

        private Image image;
        private Mock<IBitmapResamplingStrategy> resamplingStrategy;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(OriginalImageSize);
            this.resamplingStrategy = new Mock<IBitmapResamplingStrategy>();
        }

        [Test]
        public void ChangesImageSize()
        {
            Command command = new ResampleImageCommand(image, NewImageSize, resamplingStrategy.Object);

            command.Execute();
            Assert.That(image.Size, Is.EqualTo(NewImageSize));

            command.Undo();
            Assert.That(image.Size, Is.EqualTo(OriginalImageSize));
        }

        [Test]
        public void ResamplesBitmapsProportionalToImageSize()
        {
            Size originalBitmapSize = new Size(10, 20);
            FakeBitmap originalBitmap = new FakeBitmap(originalBitmapSize);

            BitmapLayer layer = new BitmapLayer(originalBitmap);
            image.Layers.Add(layer);

            Command command = new ResampleImageCommand(image, NewImageSize, resamplingStrategy.Object);

            Size expectedNewBitmapSize = originalBitmapSize * ScalingFactor;
            FakeBitmap newBitmap = new FakeBitmap(expectedNewBitmapSize);

            resamplingStrategy.Setup(rs => rs.Resample(originalBitmap, expectedNewBitmapSize)).Returns(newBitmap);

            command.Execute();
            Assert.That(layer.Bitmap, Is.EqualTo(newBitmap));

            command.Undo();
            Assert.That(layer.Bitmap, Is.EqualTo(originalBitmap));
        }
    }
}
