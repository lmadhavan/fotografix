using Fotografix.Editor;
using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class TabViewContainerTest
    {
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

            container.OpenImageEditor(CreateEmptyImageEditor);

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensImageEditorInExistingEmptyTab()
        {
            TabViewContainer container = new TabViewContainer();

            container.OpenStartPage();
            container.OpenImageEditor(CreateEmptyImageEditor);

            Assert.AreEqual(1, container.Tabs.Count);
            Assert.AreEqual(typeof(ImageEditorPage), container.Tabs[0].ContentType);
        }

        [UITestMethod]
        public void OpensMultipleImageEditorsInSeparateTabs()
        {
            TabViewContainer container = new TabViewContainer();

            container.OpenImageEditor(CreateEmptyImageEditor);
            container.OpenImageEditor(CreateEmptyImageEditor);

            Assert.AreEqual(2, container.Tabs.Count);
        }

        private ImageEditor CreateEmptyImageEditor(Viewport viewport)
        {
            return new ImageEditor(new Document());
        }
    }
}