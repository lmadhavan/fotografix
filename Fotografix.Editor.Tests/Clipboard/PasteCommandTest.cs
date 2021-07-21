using NUnit.Framework;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Editor.Clipboard
{
    [TestFixture]
    public class PasteCommandTest
    {
        private FakeClipboard clipboard;
        private PasteCommand command;

        [SetUp]
        public void SetUp()
        {
            this.clipboard = new();
            this.command = new PasteCommand(clipboard);
        }

        [Test]
        public async Task PastesClipboardContentCenteredAsNewLayer()
        {
            Image image = new Image(new Size(200, 100));
            Document document = new Document(image);

            Bitmap clipboardContent = new Bitmap(new Size(50, 50));
            clipboard.SetBitmap(clipboardContent);

            Assert.IsTrue(command.CanExecute(new Document()));

            await command.ExecuteAsync(document);

            Assert.That(image.Layers, Has.Count.EqualTo(1));

            Assert.That(image.Layers[0].Content, Is.EqualTo(clipboardContent));
            Assert.That(clipboardContent.Position, Is.EqualTo(new Point(75, 25)));
        }

        [Test]
        public void DisabledWhenClipboardDoesNotHaveBitmap()
        {
            Assert.IsFalse(command.CanExecute(new Document()));
        }
    }
}
