using Moq;
using NUnit.Framework;
using System.Drawing;

namespace Fotografix.Editor.Tools
{
    [TestFixture]
    public class CropToolTest
    {
        private static readonly Size ImageSize = new Size(100, 100);

        private Image image;
        private Document document;
        private Mock<IDocumentCommand> cropCommand;
        private CropTool tool;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(ImageSize);
            this.document = new Document(image);
            this.cropCommand = new();
            this.tool = new CropTool(cropCommand.Object);
        }

        [Test]
        public void BeginsCropPreviewWhenActivated()
        {
            tool.Activated(document);

            Assert.That(image.GetCropPreview(), Is.EqualTo(new Rectangle(Point.Empty, ImageSize)));
        }

        [Test]
        public void UpdatesCropPreviewWhenHandleMoved()
        {
            tool.Activated(document);
            tool.PointerPressed(new Point(0, 0));
            tool.PointerMoved(new Point(10, 10));

            Assert.That(image.GetCropPreview(), Is.EqualTo(Rectangle.FromLTRB(10, 10, ImageSize.Width, ImageSize.Height)));
        }

        [Test]
        public void UpdatesCropPreviewWhenImageSizeChanges()
        {
            tool.Activated(document);

            Size newImageSize = new Size(50, 50);
            image.Size = newImageSize;

            Assert.That(image.GetCropPreview(), Is.EqualTo(new Rectangle(Point.Empty, newImageSize)));
        }

        [Test]
        public void EndsCropPreviewWhenDeactivated()
        {
            tool.Activated(document);
            tool.Deactivated();

            Assert.That(image.GetCropPreview(), Is.Null);
        }

        [Test]
        public void CommitsCropOperation()
        {
            tool.Activated(document);
            tool.Commit();

            cropCommand.Verify(c => c.ExecuteAsync(document));
        }
    }
}
