using Fotografix.IO;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    [TestFixture]
    public class SaveImageCommandTest
    {
        private Mock<IImageEncoder> imageEncoder;
        private Mock<IFilePicker> filePicker;

        private Document document;
        private FileFormat fileFormat;
        private FileFormat[] supportedFormats;

        private SaveImageCommand command;

        [SetUp]
        public void SetUp()
        {
            this.imageEncoder = new Mock<IImageEncoder>();
            this.filePicker = new Mock<IFilePicker>();

            this.document = new Document { IsDirty = true };
            this.command = new SaveImageCommand(imageEncoder.Object, filePicker.Object);
        }

        [Test]
        public async Task SavePromptsForFileWhenNotPresent()
        {
            SetupEncoderFormat(".tst");

            IFile file = new InMemoryFile("test.tst");
            SetupFilePicker(file);

            await command.ExecuteAsync(document);

            filePicker.Verify(p => p.PickSaveFileAsync(supportedFormats));
            AssertImageSaved(file);
        }

        [Test]
        public async Task SaveUsesExistingFileWhenPresent()
        {
            SetupEncoderFormat(".tst");

            IFile file = new InMemoryFile("test.tst");
            document.File = file;

            await command.ExecuteAsync(document);

            filePicker.VerifyNoOtherCalls();
            AssertImageSaved(file);
        }

        [Test]
        public async Task SavePromptsForFileWhenExistingFormatIsNotSupported()
        {
            SetupEncoderFormat(".tst");

            IFile existingFile = new InMemoryFile("file.xyz");
            document.File = existingFile;

            IFile newFile = new InMemoryFile("file.tst");
            SetupFilePicker(newFile);

            await command.ExecuteAsync(document);

            filePicker.Verify(p => p.PickSaveFileAsync(supportedFormats));
            AssertImageSaved(newFile);
        }

        [Test]
        public async Task SaveIsCancelledIfNoFileIsPicked()
        {
            SetupEncoderFormat(".tst");
            SetupFilePicker(null);

            await command.ExecuteAsync(document);

            Assert.IsNull(document.File);
            AssertImageNotSaved();
        }

        [Test]
        public async Task SaveAsAlwaysPromptsForFile()
        {
            SetupEncoderFormat(".tst");

            IFile existingFile = new InMemoryFile("file1.tst");
            document.File = existingFile;

            IFile newFile = new InMemoryFile("file2.tst");
            SetupFilePicker(newFile);

            command.Mode = SaveCommandMode.SaveAs;
            await command.ExecuteAsync(document);

            filePicker.Verify(p => p.PickSaveFileAsync(supportedFormats));
            AssertImageSaved(newFile);
        }

        [Test]
        public async Task SaveAsIsCancelledIfNoFileIsPicked()
        {
            SetupEncoderFormat(".tst");

            IFile existingFile = new InMemoryFile("file.tst");
            document.File = existingFile;

            SetupFilePicker(null);

            command.Mode = SaveCommandMode.SaveAs;
            await command.ExecuteAsync(document);

            Assert.That(document.File, Is.EqualTo(existingFile));
            AssertImageNotSaved();
        }

        private void SetupEncoderFormat(string fileExtension)
        {
            this.fileFormat = new FileFormat("Test Format", fileExtension);
            this.supportedFormats = new[] { fileFormat };
            imageEncoder.SetupGet(e => e.SupportedFileFormats).Returns(supportedFormats);
        }

        private void SetupFilePicker(IFile file)
        {
            filePicker.Setup(p => p.PickSaveFileAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult(file));
        }

        private void AssertImageSaved(IFile file)
        {
            Assert.That(document.File, Is.EqualTo(file));
            Assert.IsFalse(document.IsDirty, "Dirty");
            imageEncoder.Verify(e => e.WriteImageAsync(document.Image, file, fileFormat));
        }

        private void AssertImageNotSaved()
        {
            Assert.IsTrue(document.IsDirty, "Dirty");
            imageEncoder.Verify(e => e.WriteImageAsync(It.IsAny<Image>(), It.IsAny<IFile>(), It.IsAny<FileFormat>()), Times.Never);
        }
    }
}
