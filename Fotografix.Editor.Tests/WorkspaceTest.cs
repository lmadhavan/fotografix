using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    [TestFixture]
    public class WorkspaceTest
    {
        private Document document;
        private Mock<IObservableDocumentCommand> documentCommand;

        private Workspace workspace;
        private AsyncCommand boundCommand;

        [SetUp]
        public void SetUp()
        {
            this.document = new();
            this.documentCommand = new();

            this.workspace = new();
            this.boundCommand = workspace.Bind(documentCommand.Object);
        }

        [Test]
        public void ActivatesNewlyAddedDocument()
        {
            workspace.AddDocument(document);

            Assert.That(workspace.ActiveDocument, Is.EqualTo(document));
            Assert.That(workspace.Documents, Is.EqualTo(new Document[] { document }).AsCollection);
        }

        [Test]
        public void DectivatesActiveDocumentWhenRemoved()
        {
            Document anotherDocument = new();

            workspace.AddDocument(document);
            workspace.AddDocument(anotherDocument);
            workspace.RemoveDocument(anotherDocument);

            Assert.That(workspace.ActiveDocument, Is.EqualTo(document));
            Assert.That(workspace.Documents, Is.EqualTo(new Document[] { document }).AsCollection);
        }

        [Test]
        public void DoesNotChangeActiveDocumentWhenInactiveDocumentIsRemoved()
        {
            Document anotherDocument = new();

            workspace.AddDocument(document);
            workspace.AddDocument(anotherDocument);
            workspace.RemoveDocument(document);

            Assert.That(workspace.ActiveDocument, Is.EqualTo(anotherDocument));
            Assert.That(workspace.Documents, Is.EqualTo(new Document[] { anotherDocument }).AsCollection);
        }

        [Test]
        public async Task ExecutesCommandOnActiveDocument()
        {
            documentCommand.Setup(c => c.CanExecute(It.IsAny<Document>())).Returns(true);

            workspace.ActiveDocument = document;

            Assert.IsTrue(boundCommand.CanExecute());
            documentCommand.Verify(c => c.CanExecute(document));

            await boundCommand.ExecuteAsync();

            documentCommand.Verify(c => c.ExecuteAsync(document));
        }

        [Test]
        public async Task GroupsChangesProducedByCommand()
        {
            Image image = document.Image;
            image.Size = new(10, 10);

            int changeCount = 0;
            document.ContentChanged += (s, e) => changeCount++;

            documentCommand.Setup(c => c.CanExecute(It.IsAny<Document>())).Returns(true);
            documentCommand.Setup(c => c.ExecuteAsync(It.IsAny<Document>())).Callback(() =>
            {
                image.Size = new(20, 20);
                image.Size = new(30, 30);
            });

            workspace.ActiveDocument = document;

            await boundCommand.ExecuteAsync();

            Assert.That(changeCount, Is.EqualTo(1));
        }

        [Test]
        public void CannotExecuteCommandWhenNoDocumentIsActive()
        {
            documentCommand.Setup(c => c.CanExecute(It.IsAny<Document>())).Returns(true);

            Assert.IsFalse(boundCommand.CanExecute());
        }

        [Test]
        public void CannotExecuteCommandIfNotValidForActiveDocument()
        {
            documentCommand.Setup(c => c.CanExecute(It.IsAny<Document>())).Returns(false);

            workspace.ActiveDocument = document;

            Assert.IsFalse(boundCommand.CanExecute());
        }

        [Test]
        public void RaisesCanExecuteChangedWhenActiveDocumentIsSet()
        {
            AssertCanExecuteChanged(() => workspace.ActiveDocument = document);
        }

        [Test]
        public void RaisesCanExecuteChangedWhenDocumentContentChanges()
        {
            document.Image.Size = new(10, 10);
            workspace.ActiveDocument = document;

            AssertCanExecuteChanged(() => document.Image.Size = new(20, 20));
        }

        [Test]
        public void PassesThroughCanExecuteChangedFromUnderlyingCommand()
        {
            AssertCanExecuteChanged(() => documentCommand.Raise(c => c.CanExecuteChanged += null, EventArgs.Empty));
        }

        private void AssertCanExecuteChanged(Action action)
        {
            bool canExecuteChanged = false;
            boundCommand.CanExecuteChanged += (s, e) => canExecuteChanged = true;

            action();

            Assert.IsTrue(canExecuteChanged);
        }
    }
}
