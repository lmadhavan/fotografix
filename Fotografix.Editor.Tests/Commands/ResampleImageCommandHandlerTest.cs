using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Commands
{
    [TestFixture]
    public class ResampleImageCommandHandlerTest
    {
        private const int ScalingFactor = 2;
        private static readonly Size OriginalImageSize = new Size(50, 25);
        private static readonly Size NewImageSize = OriginalImageSize * ScalingFactor;

        private Image image;
        private Mock<IBitmapResamplingStrategy> resamplingStrategy;
        private ResampleImageCommandHandler handler;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(OriginalImageSize);
            this.resamplingStrategy = new Mock<IBitmapResamplingStrategy>();
            this.handler = new ResampleImageCommandHandler(resamplingStrategy.Object);
        }

        [Test]
        public void ChangesImageSize()
        {
            handler.Handle(new ResampleImageCommand(image, NewImageSize));

            Assert.That(image.Size, Is.EqualTo(NewImageSize));
        }

        [Test]
        public void ResamplesBitmapsProportionalToImageSize()
        {
            Size originalBitmapSize = new Size(10, 20);
            Bitmap originalBitmap = new Bitmap(originalBitmapSize);

            BitmapLayer layer = new BitmapLayer(originalBitmap);
            image.Layers.Add(layer);

            Size expectedNewBitmapSize = originalBitmapSize * ScalingFactor;
            Bitmap newBitmap = new Bitmap(expectedNewBitmapSize);

            resamplingStrategy.Setup(rs => rs.Resample(originalBitmap, expectedNewBitmapSize)).Returns(newBitmap);

            handler.Handle(new ResampleImageCommand(image, NewImageSize));

            Assert.That(layer.Bitmap, Is.EqualTo(newBitmap));
        }
    }
}
