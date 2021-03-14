using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Crop
{
    [TestFixture]
    public class CropCommandHandlerTest
    {
        [Test]
        public void ClipsImageToCropRectangle()
        {
            CropCommandHandler handler = new CropCommandHandler();

            Image image = new Image(new Size(100, 100));
            Bitmap bitmap = new Bitmap(Size.Empty) { Position = new Point(10, 10) };
            Layer layer = new BitmapLayer(bitmap);
            image.Layers.Add(layer);

            handler.Handle(new CropCommand(image, new Rectangle(25, 25, 50, 50)));

            Assert.That(image.Size, Is.EqualTo(new Size(50, 50)), "Image size");
            Assert.That(bitmap.Position, Is.EqualTo(new Point(-15, -15)), "Bitmap position");
        }
    }
}
