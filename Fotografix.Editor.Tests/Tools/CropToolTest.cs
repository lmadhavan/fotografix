using Fotografix.Editor.Commands;
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
        private Mock<ICommandDispatcher> commandDispatcher;
        private CropTool tool;

        [SetUp]
        public void SetUp()
        {
            this.image = new Image(ImageSize);
            this.commandDispatcher = new Mock<ICommandDispatcher>();
            this.tool = new CropTool();

            image.SetCommandDispatcher(commandDispatcher.Object);
        }

        [Test]
        public void BeginsCropPreviewWhenActivated()
        {
            tool.Activated(image);

            Assert.That(image.GetCropPreview(), Is.EqualTo(new Rectangle(Point.Empty, ImageSize)));
        }

        [Test]
        public void UpdatesCropPreviewWhenHandleMoved()
        {
            tool.Activated(image);
            tool.PointerPressed(new Point(0, 0));
            tool.PointerMoved(new Point(10, 10));

            Assert.That(image.GetCropPreview(), Is.EqualTo(Rectangle.FromLTRB(10, 10, ImageSize.Width, ImageSize.Height)));
        }

        [Test]
        public void UpdatesCropPreviewWhenImageSizeChanges()
        {
            tool.Activated(image);

            Size newImageSize = new Size(50, 50);
            image.Size = newImageSize;

            Assert.That(image.GetCropPreview(), Is.EqualTo(new Rectangle(Point.Empty, newImageSize)));
        }

        [Test]
        public void EndsCropPreviewWhenDeactivated()
        {
            tool.Activated(image);
            tool.Deactivated();

            Assert.That(image.GetCropPreview(), Is.Null);
        }

        [Test]
        public void CommitsCropOperation()
        {
            tool.Activated(image);
            tool.Commit();

            commandDispatcher.Verify(d => d.DispatchAsync(new CropCommand(image, new Rectangle(Point.Empty, ImageSize))));
        }
    }
}
