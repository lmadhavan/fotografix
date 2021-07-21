using Fotografix.IO;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    [TestFixture]
    public class SaveImageCommandTest
    {
        private FakeImageCodec imageCodec;
        private FakeFilePicker filePicker;

        private Document document;
        private SaveImageCommand command;

        [SetUp]
        public void SetUp()
        {
            this.imageCodec = new();
            this.filePicker = new();

            FileFormat fileFormat = new("Test Format", ".tst");
            imageCodec.SupportedFileFormats = new[] { fileFormat };

            this.document = new Document { IsDirty = true };
            this.command = new SaveImageCommand(imageCodec, filePicker);
        }

        [Test]
        public async Task SavePromptsForFileWhenNotPresent()
        {
            IFile file = new InMemoryFile("test.tst");
            filePicker.SaveFileResult = file;

            await command.ExecuteAsync(document);

            AssertImageSaved(file);
        }

        [Test]
        public async Task SaveUsesExistingFileWhenPresent()
        {
            IFile file = new InMemoryFile("test.tst");
            document.File = file;

            await command.ExecuteAsync(document);

            AssertImageSaved(file);
        }

        [Test]
        public async Task SavePromptsForFileWhenExistingFormatIsNotSupported()
        {
            IFile existingFile = new InMemoryFile("file.xyz");
            document.File = existingFile;

            IFile newFile = new InMemoryFile("file.tst");
            filePicker.SaveFileResult = newFile;

            await command.ExecuteAsync(document);

            AssertImageSaved(newFile);
        }

        [Test]
        public async Task SaveIsCancelledIfNoFileIsPicked()
        {
            await command.ExecuteAsync(document);

            Assert.IsNull(document.File);
            AssertImageNotSaved();
        }

        [Test]
        public async Task SaveAsAlwaysPromptsForFile()
        {
            IFile existingFile = new InMemoryFile("file1.tst");
            document.File = existingFile;

            IFile newFile = new InMemoryFile("file2.tst");
            filePicker.SaveFileResult = newFile;

            command.Mode = SaveCommandMode.SaveAs;
            await command.ExecuteAsync(document);

            AssertImageSaved(newFile);
        }

        [Test]
        public async Task SaveAsIsCancelledIfNoFileIsPicked()
        {
            IFile existingFile = new InMemoryFile("file.tst");
            document.File = existingFile;

            command.Mode = SaveCommandMode.SaveAs;
            await command.ExecuteAsync(document);

            Assert.That(document.File, Is.EqualTo(existingFile));
            AssertImageNotSaved();
        }

        private void AssertImageSaved(IFile file)
        {
            Assert.That(document.File, Is.EqualTo(file));
            Assert.IsFalse(document.IsDirty, "Dirty");
            Assert.That(imageCodec.SavedImages[file], Is.EqualTo(document.Image));
        }

        private void AssertImageNotSaved()
        {
            Assert.IsTrue(document.IsDirty, "Dirty");
            Assert.That(imageCodec.SavedImages, Is.Empty);
        }
    }
}
