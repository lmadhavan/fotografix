using Fotografix.IO;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Fotografix.Editor.Commands
{
    [TestFixture]
    public class SaveCommandHandlerTest
    {
        private Mock<IImageEncoder> imageEncoder;
        private Mock<IFilePicker> filePicker;
        private SaveCommandHandler handler;

        private Image image;
        private FileFormat fileFormat;
        private FileFormat[] supportedFormats;

        [SetUp]
        public void SetUp()
        {
            this.imageEncoder = new Mock<IImageEncoder>();
            this.filePicker = new Mock<IFilePicker>();
            this.handler = new SaveCommandHandler(imageEncoder.Object, filePicker.Object);

            this.image = new Image(new Size(10, 10));
        }

        [Test]
        public async Task SavePromptsForFileWhenNotPresent()
        {
            SetupEncoderFormat(".tst");

            IFile file = new InMemoryFile("test.tst");
            filePicker.Setup(p => p.PickSaveFileAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult(file));

            await handler.HandleAsync(new SaveCommand(image));

            Assert.That(image.GetFile(), Is.EqualTo(file));
            filePicker.Verify(p => p.PickSaveFileAsync(supportedFormats));
            imageEncoder.Verify(e => e.WriteImageAsync(image, file, fileFormat));
        }

        [Test]
        public async Task SaveUsesExistingFileWhenPresent()
        {
            SetupEncoderFormat(".tst");

            IFile file = new InMemoryFile("test.tst");
            image.SetFile(file);

            await handler.HandleAsync(new SaveCommand(image));

            filePicker.VerifyNoOtherCalls();
            imageEncoder.Verify(e => e.WriteImageAsync(image, file, fileFormat));
        }

        [Test]
        public async Task SavePromptsForFileWhenExistingFormatIsNotSupported()
        {
            SetupEncoderFormat(".tst");

            IFile existingFile = new InMemoryFile("file.xyz");
            image.SetFile(existingFile);

            IFile newFile = new InMemoryFile("file.tst");
            filePicker.Setup(p => p.PickSaveFileAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult(newFile));

            await handler.HandleAsync(new SaveCommand(image));

            filePicker.Verify(p => p.PickSaveFileAsync(supportedFormats));
            imageEncoder.Verify(e => e.WriteImageAsync(image, newFile, fileFormat));
        }

        [Test]
        public async Task SaveIsCancelledIfNoFileIsPicked()
        {
            SetupEncoderFormat(".tst");

            filePicker.Setup(p => p.PickSaveFileAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult<IFile>(null));

            await handler.HandleAsync(new SaveCommand(image));

            Assert.That(image.GetFile(), Is.Null);
            imageEncoder.Verify(e => e.WriteImageAsync(It.IsAny<Image>(), It.IsAny<IFile>(), It.IsAny<FileFormat>()), Times.Never);
        }

        [Test]
        public async Task SaveAsAlwaysPromptsForFile()
        {
            SetupEncoderFormat(".tst");

            IFile existingFile = new InMemoryFile("file1.tst");
            image.SetFile(existingFile);

            IFile newFile = new InMemoryFile("file2.tst");
            filePicker.Setup(p => p.PickSaveFileAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult(newFile));

            await handler.HandleAsync(new SaveAsCommand(image));

            filePicker.Verify(p => p.PickSaveFileAsync(supportedFormats));
            imageEncoder.Verify(e => e.WriteImageAsync(image, newFile, fileFormat));
        }

        [Test]
        public async Task SaveAsIsCancelledIfNoFileIsPicked()
        {
            SetupEncoderFormat(".tst");

            IFile existingFile = new InMemoryFile("file.tst");
            image.SetFile(existingFile);

            filePicker.Setup(p => p.PickSaveFileAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult<IFile>(null));

            await handler.HandleAsync(new SaveAsCommand(image));

            Assert.That(image.GetFile(), Is.EqualTo(existingFile));
            imageEncoder.Verify(e => e.WriteImageAsync(It.IsAny<Image>(), It.IsAny<IFile>(), It.IsAny<FileFormat>()), Times.Never);
        }

        private void SetupEncoderFormat(string fileExtension)
        {
            this.fileFormat = new FileFormat("Test Format", fileExtension);
            this.supportedFormats = new[] { fileFormat };
            imageEncoder.SetupGet(e => e.SupportedFileFormats).Returns(supportedFormats);
        }
    }
}
