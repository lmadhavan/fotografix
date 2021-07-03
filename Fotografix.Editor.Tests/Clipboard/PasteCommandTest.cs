using Moq;
using NUnit.Framework;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Editor.Clipboard
{
    [TestFixture]
    public class PasteCommandTest
    {
        private Mock<IClipboard> clipboard;
        private PasteCommand command;

        [SetUp]
        public void SetUp()
        {
            this.clipboard = new Mock<IClipboard>();
            this.command = new PasteCommand(clipboard.Object);
        }

        [Test]
        public async Task PastesClipboardContentCenteredAsNewLayer()
        {
            Image image = new Image(new Size(200, 100));
            Document document = new Document(image);

            Bitmap clipboardContent = new Bitmap(new Size(50, 50));
            clipboard.SetupGet(c => c.HasBitmap).Returns(true);
            clipboard.Setup(c => c.GetBitmapAsync()).Returns(Task.FromResult(clipboardContent));

            Assert.IsTrue(command.CanExecute(new Document()));

            await command.ExecuteAsync(document);

            Assert.That(image.Layers, Has.Count.EqualTo(1));

            Assert.That(image.Layers[0].Content, Is.EqualTo(clipboardContent));
            Assert.That(clipboardContent.Position, Is.EqualTo(new Point(75, 25)));
        }

        [Test]
        public void DisabledWhenClipboardDoesNotHaveBitmap()
        {
            clipboard.SetupGet(c => c.HasBitmap).Returns(false);

            Assert.IsFalse(command.CanExecute(new Document()));
        }
    }
}
