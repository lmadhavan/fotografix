using Fotografix.Editor.Tools;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fotografix.Editor
{
    [TestFixture]
    public class WorkspaceTest
    {
        private Document document;
        private Mock<EditorCommand> editorCommand;
        private Mock<ITool> tool;

        private Workspace workspace;
        private AsyncCommand boundCommand;

        [SetUp]
        public void SetUp()
        {
            this.document = new();
            this.editorCommand = new();
            this.tool = new();

            this.workspace = new();
            this.boundCommand = workspace.Bind(editorCommand.Object);
        }

        [Test]
        public void ActivatesToolWhenActiveDocumentIsSet()
        {
            workspace.ActiveTool = tool.Object;
            workspace.ActiveDocument = document;

            tool.Verify(t => t.Activated(document));
            tool.VerifyNoOtherCalls();
        }

        [Test]
        public void DeactivatesToolWhenActiveDocumentIsReset()
        {
            workspace.ActiveDocument = document;
            workspace.ActiveTool = tool.Object;
            workspace.ActiveDocument = null;

            tool.Verify(t => t.Deactivated());
        }

        [Test]
        public void ActivatesToolWhenActiveToolIsSet()
        {
            workspace.ActiveDocument = document;
            workspace.ActiveTool = tool.Object;

            tool.Verify(t => t.Activated(document));
            tool.VerifyNoOtherCalls();
        }

        [Test]
        public void DeactivatesToolWhenActiveToolIsReset()
        {
            workspace.ActiveDocument = document;
            workspace.ActiveTool = tool.Object;
            workspace.ActiveTool = null;

            tool.Verify(t => t.Deactivated());
        }

        [Test]
        public void DoesNotDeactivateToolWhenNoDocumentIsActive()
        {
            workspace.ActiveTool = tool.Object;
            workspace.ActiveTool = null;

            tool.VerifyNoOtherCalls();
        }

        [Test]
        public void DoesNotAttemptToDeactivateWhenNoToolIsActive()
        {
            workspace.ActiveDocument = document;
            workspace.ActiveDocument = null;

            Assert.Pass();
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
        public async Task BindsEditorCommandToWorkspace()
        {
            object parameter = new();
            editorCommand.Setup(c => c.CanExecute(It.IsAny<Workspace>(), It.IsAny<object>())).Returns(true);

            Assert.IsTrue(boundCommand.CanExecute(parameter));
            editorCommand.Verify(c => c.CanExecute(workspace, parameter));

            await boundCommand.ExecuteAsync(parameter);

            editorCommand.Verify(c => c.ExecuteAsync(workspace, parameter, It.IsAny<CancellationToken>(), It.IsAny<IProgress<EditorCommandProgress>>()));
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

        private void AssertCanExecuteChanged(Action action)
        {
            bool canExecuteChanged = false;
            boundCommand.CanExecuteChanged += (s, e) => canExecuteChanged = true;

            action();

            Assert.IsTrue(canExecuteChanged);
        }
    }
}
