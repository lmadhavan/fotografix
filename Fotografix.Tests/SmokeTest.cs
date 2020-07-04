﻿using Fotografix.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;

namespace Fotografix.Tests
{
    [TestClass]
    public class SmokeTest
    {
        [UITestMethod]
        public void LoadsUI()
        {
            TabViewContainer container = new TabViewContainer();
            Window.Current.Content = container;
            container.OpenStartPage();
        }
    }
}