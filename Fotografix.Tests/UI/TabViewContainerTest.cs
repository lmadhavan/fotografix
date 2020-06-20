using Fotografix.UI;
using Fotografix.UI.FileManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Drawing;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class TabViewContainerTest
    {
        private readonly ICreateImageEditorCommand command = new NewImageCommand(new Size(100, 100));

        [UITestMethod]
        public void OpensStartPage()
        {
            TabViewContainer container = new TabViewContainer();

            container.OpenStartPage();

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(StartPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensImageEditor()
        {
            TabViewContainer container = new TabViewContainer();

            container.OpenImageEditor(command);

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensImageEditorInExistingEmptyTab()
        {
            TabViewContainer container = new TabViewContainer();

            container.OpenStartPage();
            container.OpenImageEditor(command);

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensMultipleImageEditorsInSeparateTabs()
        {
            TabViewContainer container = new TabViewContainer();

            container.OpenImageEditor(command);
            container.OpenImageEditor(command);

            Assert.AreEqual(2, container.Tabs.Count);
        }
    }
}