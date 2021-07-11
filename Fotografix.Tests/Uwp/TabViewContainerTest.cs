using Fotografix.Editor;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class TabViewContainerTest
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
            TabViewContainer container = new TabViewContainer(workspace);

            container.OpenStartPage();

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(StartPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensDocumentInNewTab()
        {
            TabViewContainer container = new TabViewContainer(workspace);

            workspace.AddDocument(new Document());

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensDocumentInActiveTabIfEmpty()
        {
            TabViewContainer container = new TabViewContainer(workspace);

            container.OpenStartPage();
            workspace.AddDocument(new Document());

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensMultipleDocumentsInSeparateTabs()
        {
            TabViewContainer container = new TabViewContainer(workspace);

            workspace.AddDocument(new Document());
            workspace.AddDocument(new Document());

            Assert.AreEqual(2, container.Tabs.Count);
        }

        [UITestMethod]
        public void SyncsActiveTabWithActiveDocument()
        {
            TabViewContainer container = new TabViewContainer(workspace);

            Document document1 = new Document();
            Document document2 = new Document();

            workspace.AddDocument(document1);
            Tab tab1 = container.ActiveTab;

            workspace.AddDocument(document2);
            Tab tab2 = container.ActiveTab;

            container.ActiveTab = tab1;

            Assert.AreEqual(document1, workspace.ActiveDocument, "document1 should be active document");

            workspace.RemoveDocument(document1);

            Assert.AreEqual(tab2, container.ActiveTab, "tab2 should be active tab");

            workspace.RemoveDocument(document2);

            Assert.IsNull(container.ActiveTab, "there should be no active tab");
        }
    }
}