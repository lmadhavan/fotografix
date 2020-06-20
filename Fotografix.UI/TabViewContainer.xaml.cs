using Fotografix.UI.FileManagement;
using Microsoft.UI.Xaml.Controls;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Fotografix.UI
{
    public sealed partial class TabViewContainer : UserControl, ICustomTitleBarProvider
    {
        private readonly Workspace workspace;

        public TabViewContainer()
        {
            this.InitializeComponent();
            this.Tabs = new TabCollection(tabView.TabItems);

            this.workspace = new Workspace();
            workspace.OpenImageEditorRequested += (s, e) => OpenImageEditor(e.Command);
        }

        public IReadOnlyList<Tab> Tabs { get; }

        public void OpenStartPage()
        {
            CreateEmptyTab().OpenStartPage(workspace);
        }

        public void OpenImageEditor(ICreateImageEditorCommand command)
        {
            GetOrCreateEmptyTab().OpenImageEditor(command);
        }

        private Tab GetOrCreateEmptyTab()
        {
            Tab tab = (Tab)tabView.SelectedItem;

            if (tab != null && tab.IsEmpty)
            {
                return tab;
            }

            return CreateEmptyTab();
        }

        private Tab CreateEmptyTab()
        {
            Tab tab = new Tab();
            tabView.TabItems.Add(tab);
            tabView.SelectedItem = tab;

            return tab;
        }

        #region Tab collection wrapper
        private sealed class TabCollection : IReadOnlyList<Tab>
        {
            private readonly IList<object> tabs;

            public TabCollection(IList<object> tabs)
            {
                this.tabs = tabs;
            }

            public int Count => tabs.Count;

            public Tab this[int index] => (Tab)tabs[index];

            public IEnumerator<Tab> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        #endregion

        #region TabView event handlers

        private void OnNewTabRequested(TabView sender, object args)
        {
            OpenStartPage();
        }

        private void OnCloseTabRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            Tab tab = (Tab)args.Tab;
            tabView.TabItems.Remove(tab);
            tab.Dispose();
        }

        #endregion

        #region Keyboard accelerator event handlers

        private async void OnNewImageInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            await workspace.NewImageAsync();
        }

        private async void OnOpenFileInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            args.Handled = true;
            await workspace.OpenFileAsync();
        }

        #endregion

        #region Title bar customization

        UIElement ICustomTitleBarProvider.CustomTitleBar => tabStripFooter;

        void ICustomTitleBarProvider.UpdateLayout(ITitleBarLayoutMetrics metrics)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                tabStripHeader.MinWidth = metrics.SystemOverlayLeftInset;
                tabStripFooter.MinWidth = metrics.SystemOverlayRightInset;
            }
            else
            {
                tabStripHeader.MinWidth = metrics.SystemOverlayRightInset;
                tabStripFooter.MinWidth = metrics.SystemOverlayLeftInset;
            }

            tabStripHeader.Height = tabStripFooter.Height = metrics.Height;
        }

        #endregion
    }
}
