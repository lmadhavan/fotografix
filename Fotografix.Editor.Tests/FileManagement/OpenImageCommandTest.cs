using Fotografix.IO;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Fotografix.Editor.FileManagement
{
    [TestFixture]
    public class OpenImageCommandTest
    {
        private FakeFilePicker filePicker;
        private FakeImageCodec imageCodec;

        private OpenImageCommand command;

        [SetUp]
        public void SetUp()
        {
            this.filePicker = new();
            this.imageCodec = new();
            this.command = new(imageCodec, filePicker);
        }

        [Test]
        public async Task OpensImageAsDocumentInWorkspace()
        {
            IFile file = new InMemoryFile("file");

            Image image = new();
            imageCodec.SavedImages[file] = image;

            filePicker.OpenFilesResult = new[] { file };

            Workspace workspace = new();
            await command.ExecuteAsync(workspace);

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

            filePicker.OpenFilesResult = new[] { file2 };

            await command.ExecuteAsync(workspace);

            Assert.That(imageCodec.ReadCount, Is.Zero, "# of file reads");
            Assert.That(workspace.Documents, Has.Count.EqualTo(2), "# of documents");
            Assert.That(workspace.ActiveDocument, Is.EqualTo(document2));
        }
    }
}
