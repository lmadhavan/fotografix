using Fotografix.Editor;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class WorkspaceViewTest
    {
        private Workspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            this.workspace = new Workspace();
        }

        [UITestMethod]
        public void OpensStartPage()
        {
            WorkspaceView workspaceView = new WorkspaceView(workspace);

            workspaceView.OpenStartPage();

            Assert.AreEqual(1, workspaceView.Tabs.Count);
            Assert.AreEqual(typeof(StartPage), workspaceView.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensDocumentInNewTab()
        {
            WorkspaceView workspaceView = new WorkspaceView(workspace);

            workspace.AddDocument(new Document());

            Assert.AreEqual(1, workspaceView.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), workspaceView.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensDocumentInActiveTabIfEmpty()
        {
            WorkspaceView workspaceView = new WorkspaceView(workspace);

            workspaceView.OpenStartPage();
            workspace.AddDocument(new Document());

            Assert.AreEqual(1, workspaceView.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), workspaceView.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensMultipleDocumentsInSeparateTabs()
        {
            WorkspaceView workspaceView = new WorkspaceView(workspace);

            workspace.AddDocument(new Document());
            workspace.AddDocument(new Document());

            Assert.AreEqual(2, workspaceView.Tabs.Count);
        }

        [UITestMethod]
        public void SyncsActiveTabWithActiveDocument()
        {
            WorkspaceView workspaceView = new WorkspaceView(workspace);

            Document document1 = new Document();
            Document document2 = new Document();

            workspace.AddDocument(document1);
            Tab tab1 = workspaceView.ActiveTab;

            workspace.AddDocument(document2);
            Tab tab2 = workspaceView.ActiveTab;

            workspaceView.ActiveTab = tab1;

            Assert.AreEqual(document1, workspace.ActiveDocument, "document1 should be active document");

            workspace.RemoveDocument(document1);

            Assert.AreEqual(tab2, workspaceView.ActiveTab, "tab2 should be active tab");

            workspace.RemoveDocument(document2);

            Assert.IsNull(workspaceView.ActiveTab, "there should be no active tab");
        }
    }
}