﻿using Fotografix.Uwp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;

namespace Fotografix.Tests.Uwp
{
    [TestClass]
    public class SmokeTest
    {
        [UITestMethod]
        public void LoadsUI()
        {
            WorkspaceView workspaceView = new WorkspaceView();
            Window.Current.Content = workspaceView;
            workspaceView.OpenStartPage();
        }
    }
}
