using Fotografix.IO;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    [TestFixture]
    public class OpenImageCommandTest
    {
        private IEnumerable<FileFormat> fileFormats;
        private Mock<IFilePicker> filePicker;
        private Mock<IImageDecoder> imageDecoder;

        private OpenImageCommand command;

        [SetUp]
        public void SetUp()
        {
            this.fileFormats = new FileFormat[] { new FileFormat("Test", ".tst") };
            this.filePicker = new();

            this.imageDecoder = new();
            imageDecoder.SetupGet(d => d.SupportedFileFormats).Returns(fileFormats);

            this.command = new(imageDecoder.Object, filePicker.Object);
        }

        [Test]
        public async Task OpensImageAsDocumentInWorkspace()
        {
            IFile file = new InMemoryFile("file");

            Image image = new();
            imageDecoder.Setup(d => d.ReadImageAsync(file)).Returns(Task.FromResult(image));

            SetupFilePicker(file);

            Workspace workspace = new();
            await command.ExecuteAsync(workspace);

            filePicker.Verify(p => p.PickOpenFilesAsync(fileFormats));

            Assert.That(workspace.Documents, Has.Count.EqualTo(1));

            Document document = workspace.Documents.First();
            Assert.That(document.Image, Is.EqualTo(image));
            Assert.That(document.File, Is.EqualTo(file));
        }

        [Test]
        public async Task ActivatesExistingDocumentWhenFileIsReopened()
        {
            IFile file1 = new InMemoryFile("file1");
            IFile file2 = new InMemoryFile("file2");

            Document document1 = new Document { File = file1 };
            Document document2 = new Document { File = file2 };

            Workspace workspace = new();
            workspace.AddDocument(document1);
            workspace.AddDocument(document2);
            workspace.ActiveDocument = document1;

            SetupFilePicker(file2);

            await command.ExecuteAsync(workspace);

            imageDecoder.Verify(d => d.ReadImageAsync(It.IsAny<IFile>()), Times.Never);

            Assert.That(workspace.Documents, Has.Count.EqualTo(2));
            Assert.That(workspace.ActiveDocument, Is.EqualTo(document2));
        }

        private void SetupFilePicker(IFile file)
        {
            IEnumerable<IFile> files = new IFile[] { file };
            filePicker.Setup(p => p.PickOpenFilesAsync(It.IsAny<IEnumerable<FileFormat>>())).Returns(Task.FromResult(files));
        }
    }
}
