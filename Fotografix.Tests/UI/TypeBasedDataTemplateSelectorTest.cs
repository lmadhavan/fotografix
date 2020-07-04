using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System;
using Windows.UI.Xaml;

namespace Fotografix.Tests.UI
{
    [TestClass]
    public class TypeBasedDataTemplateSelectorTest
    {
        private DataTemplate template;
        private TypeBasedDataTemplateSelector selector;

        [UITestMethod]
        public void ReturnsTemplateThatMatchesInterfaceOfInputObject()
        {
            SetupSelectorFor<TestInterface>();

            Assert.AreEqual(template, selector.SelectTemplate(new TestClass()));
        }

        [UITestMethod]
        public void ThrowsExceptionIfNoTemplateMatches()
        {
            SetupSelectorFor<TestInterface>();

            Assert.ThrowsException<ArgumentException>(() => selector.SelectTemplate(new object()));
        }

        [UITestMethod]
        public void ReturnsNullIfInputIsNull()
        {
            SetupSelectorFor<TestInterface>();

            Assert.IsNull(selector.SelectTemplate(null));
        }
        
        private void SetupSelectorFor<T>()
        {
            this.template = new DataTemplate();

            this.selector = new TypeBasedDataTemplateSelector();
            selector.SetTemplate<T>(template);
        }

        private interface TestInterface { }
        private class TestClass : TestInterface { }
    }
}
