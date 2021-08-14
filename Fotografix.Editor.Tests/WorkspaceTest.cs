using Fotografix.Editor.Tools;
using Moq;
using NUnit.Framework;

namespace Fotografix.Editor
{
    [TestFixture]
    public class WorkspaceTest
    {
        private Document document;
        private Mock<ITool> tool;
        private Workspace workspace;

        [SetUp]
        public void SetUp()
        {
            this.document = new();
            this.tool = new();
            this.workspace = new();
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
    }
}
